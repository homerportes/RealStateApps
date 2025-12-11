using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;

namespace RealStateApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("administracion/desarrolladores")]
    [Authorize(Roles = "ADMIN")]

    public class DevelopersController : Controller
    {
        private readonly IAdministrationService _administrationService;
        private readonly IMapper _mapper;
        private readonly IAccountServiceForWebApp _accountService;

        public DevelopersController(IAdministrationService administrationService, IMapper mapper, IAccountServiceForWebApp accountService)
        {
            _administrationService = administrationService;
            _mapper = mapper;
            _accountService = accountService;
        }

        // INDEX
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var developers = await _administrationService.GetDevelopers();
            var vms = _mapper.Map<List<UserViewModel>>(developers);
            return View("Index", vms);
        }

        // CREATE GET
        [HttpGet("crear")]
        public IActionResult Create()
        {
            return View("Save", new SaveBasicUserViewModel() { ConfirmPassword="", Dni="", Email="", FirstName="", LastName="", Password="", Role="", UserName=""});
        }

        // CREATE POST
        [HttpPost("crear")]
        public async Task<IActionResult> Create(SaveBasicUserViewModel vm)
        {
            vm.Role = "DEVELOPER";

            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Roles.Add("DEVELOPER");

            var origin = Request.Headers["Origin"].ToString();
            var result = await _accountService.RegisterAsync(dto, origin);

            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Errors.First();
                return View("Save", vm);
            }

            return RedirectToAction("Index");
        }

        [HttpGet("editar/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var admin = await _accountService.GetById(id);
            if (admin == null) return NotFound();

            var vm = _mapper.Map<SaveBasicUserViewModel>(admin);

            vm.Password = "";
            vm.ConfirmPassword = "";
            return View("Save", vm);
        }

        [HttpPost("editar")]
        public async Task<IActionResult> Edit(SaveBasicUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Save", vm);

            var dto = _mapper.Map<SaveUserDto>(vm);
           var result= await _accountService.EditUser(dto);
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Errors.First();
                return View("Save", vm);

            }
            return RedirectToAction("Index");
        }

        [HttpGet("desactivar/{id}")]
        public async Task<IActionResult> Deactivate(string id)
        {
            var user = await _accountService.GetById(id);

            ViewBag.IsActivateMode = false;
            ViewBag.UserName = $"{user!.FirstName} {user.LastName}";
            return View("ChangeState", id);
        }

        [HttpGet("activar/{id}")]
        public async Task<IActionResult> Activate(string id)
        {
            var user = await _accountService.GetById(id);

            ViewBag.IsActivateMode = true;
            ViewBag.UserName = $"{user!.FirstName} {user.LastName}";
            return View("ChangeState", id);
        }

        [HttpPost("cambiar-estado/{id}")]
        public async Task<IActionResult> ChangeState(string id)
        {
            await _administrationService.ToogleState(id);
            return RedirectToAction("Index");
        }
    }
}
