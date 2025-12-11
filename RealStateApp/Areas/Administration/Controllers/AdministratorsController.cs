using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.User;

[Area("Administration")]
[Authorize(Roles = "ADMIN")]
public class AdministratorsController : Controller
{
    private readonly IAdministrationService _administrationService;
    private readonly IAccountServiceForWebApp _accountService;
    private readonly IMapper _mapper;

    public AdministratorsController(IAdministrationService administrationService,
        IAccountServiceForWebApp accountService,
        IMapper mapper)
    {
        _administrationService = administrationService;
        _accountService = accountService;
        _mapper = mapper;
    }

    // LISTADO
    [HttpGet("administradores")]
    public async Task<IActionResult> Index()
    {
        var administrators = await _administrationService.GetAdministrators();
        var vms = _mapper.Map<List<UserViewModel>>(administrators);

        var currentUserName = User.Identity?.Name;
        var currentUser = await _accountService.GetByUserName(currentUserName ?? "");
        ViewBag.CurrentUserId = currentUser!.Id;

        return View(vms);
    }

    // CREAR
    [HttpGet("administradores/crear")]
    public IActionResult Create()
    {
        return View("Save", new SaveBasicUserViewModel()
        {
            ConfirmPassword = "",
            Dni = "",
            Email = "",
            FirstName = "",
            LastName = "",
            Password = "",
            Role = "ADMIN",
            UserName = ""
        });
    }

    [HttpPost("administradores/crear")]
    public async Task<IActionResult> Create(SaveBasicUserViewModel vm)
    {
        vm.Role = "ADMIN";

        var dto = _mapper.Map<SaveUserDto>(vm);
        dto.Roles.Add("ADMIN");

        var origin = Request.Headers["Origin"].ToString();
        var result = await _accountService.RegisterAsync(dto, origin);

        if (result.HasError)
        {
            ViewBag.ErrorMessage = result.Errors.First();
            return View("Save", vm);
        }

        return RedirectToAction("Index");
    }

    [HttpGet("administradores/editar/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var admin = await _accountService.GetById(id);
        if (admin == null) return NotFound();

        var vm = _mapper.Map<SaveBasicUserViewModel>(admin);

        vm.Password = "";
        vm.ConfirmPassword = "";
        return View("Save", vm);
    }

    [HttpPost("administradores/editar/{id}")]
    public async Task<IActionResult> Edit(string id, SaveBasicUserViewModel vm)
    {
        vm.Id = id;

        var dto = _mapper.Map<SaveUserDto>(vm);
        dto.Roles.Add("ADMIN");

        var result = await _accountService.EditUser(dto);

        if (result.HasError)
        {
            ViewBag.ErrorMessage = result.Errors.First();
            return View("Save", vm);
        }

        return RedirectToAction("Index");
    }

    public IActionResult Deactivate(string id)
    {
        ViewBag.IsActivateMode = false;
        return View("ChangeState", id);
    }

    public IActionResult Activate(string id)
    {
        ViewBag.IsActivateMode = true;
        return View("ChangeState", id);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> ToggleState(string id)
    {
        await _administrationService.ToogleState(id);
        return RedirectToAction("Index");
    }
}