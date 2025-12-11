using AutoMapper;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;
using RealStateApp.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RealStateApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public LoginController(IAccountServiceForWebApp accountServiceForWebApp, UserManager<AppUser> userManager, IMapper mapper)
        {
            _accountServiceForWebApp = accountServiceForWebApp;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string message = null!)
        {
            if (!string.IsNullOrEmpty(message))
                TempData["ErrorMessage"] = message;

            // Si ya hay un usuario autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                AppUser? userSession = await _userManager.GetUserAsync(User);

                if (userSession != null)
                {
                    // Verificar que el usuario esté activo y confirmado
                    if (userSession.IsActive && userSession.EmailConfirmed)
                    {
                        var roles = await _userManager.GetRolesAsync(userSession);
                        if (roles.Any())
                        {
                            return RedirectToHome(roles.First());
                        }
                    }

                    // Si el usuario no está activo o no tiene email confirmado, cerrar sesión
                    await _accountServiceForWebApp.SignOutAsync();
                }
            }

            return View(new LoginViewModel() { Password = "", UserName = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            // Si ya hay un usuario autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                AppUser? userSession = await _userManager.GetUserAsync(User);

                if (userSession != null)
                {
                    // Verificar que el usuario esté activo y confirmado
                    if (userSession.IsActive && userSession.EmailConfirmed)
                    {
                        var roles = await _userManager.GetRolesAsync(userSession);
                        if (roles.Any())
                        {
                            return RedirectToHome(roles.First());
                        }
                    }

                    // Si el usuario no está activo o no tiene email confirmado, cerrar sesión
                    await _accountServiceForWebApp.SignOutAsync();
                }
            }

            if (!ModelState.IsValid)
            {
                vm.Password = "";
                return View(vm);
            }

            // Validaciones adicionales
            if (string.IsNullOrWhiteSpace(vm.UserName))
            {
                vm.HasError = true;
                vm.Error = "El nombre de usuario no puede estar vacío o contener solo espacios";
                vm.Password = "";
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                vm.HasError = true;
                vm.Error = "La contraseña no puede estar vacía o contener solo espacios";
                vm.Password = "";
                return View(vm);
            }

            if (vm.UserName.Length < 3)
            {
                vm.HasError = true;
                vm.Error = "El nombre de usuario debe tener al menos 3 caracteres";
                vm.Password = "";
                return View(vm);
            }

            if (vm.Password.Length < 6)
            {
                vm.HasError = true;
                vm.Error = "La contraseña debe tener al menos 6 caracteres";
                vm.Password = "";
                return View(vm);
            }

            LoginResponseDto userDto = await _accountServiceForWebApp.AuthenticateAsync(_mapper.Map<LoginDto>(vm));

            if (userDto != null && !userDto.HasError)
            {
                if (userDto.Roles != null && userDto.Roles.Any())
                {
                    return RedirectToHome(userDto.Roles.First());
                }
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }
            else
            {
                vm.HasError = true;
                vm.Error = userDto?.Error ?? "Error al iniciar sesión";
                vm.Password = "";
                return View(vm);
            }
        }

        private IActionResult RedirectToHome(string role)
        {
            return role.ToUpper() switch
            {
                "ADMIN" => RedirectToAction("Index", "Home", new { area = "Administration" })
,
                "AGENT" => RedirectToRoute(new { area = "Agents", controller = "Home", action = "Index" }),
                "CLIENT" => RedirectToRoute(new { area = "Clients", controller = "Home", action = "Index" }),
                _ => RedirectToRoute(new { controller = "Login", action = "Index" })
            };
        }

        public async Task<IActionResult> Logout()
        {
            await _accountServiceForWebApp.SignOutAsync();
            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            UserResponseDto response = await _accountServiceForWebApp.ConfirmAccountAsync(token, userId);
            return View("ConfirmEmail", response.Message);
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel() { UserName = "" });
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string origin = $"{Request.Scheme}://{Request.Host}";

            var dto = _mapper.Map<ForgotPasswordRequestDto>(vm);
            dto.Origin = origin;

            UserResponseDto returnUser = await _accountServiceForWebApp.ForgotPasswordAsync(dto);

            if (returnUser.HasError)
            {
                vm.HasError = true;
                vm.Error = string.Join(", ", returnUser.Errors ?? new List<string>());
                return View(vm);
            }

            TempData["Success"] = "Se ha enviado un correo con las instrucciones para restablecer tu contraseña.";
            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordViewModel() { UserId = userId, Token = token, Password = "", ConfirmPassword = "" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Password = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }

            var dto = _mapper.Map<ResetPasswordRequestDto>(vm);

            UserResponseDto returnUser = await _accountServiceForWebApp.ResetPasswordAsync(dto);

            if (returnUser.HasError)
            {
                vm.HasError = true;
                vm.Error = string.Join(", ", returnUser.Errors ?? new List<string>());
                vm.Password = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }

            TempData["Success"] = "Tu contraseña ha sido restablecida exitosamente.";
            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }
    }
}
