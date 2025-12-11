using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.ViewModels.Dashboards;
using System.Threading.Tasks;

namespace RealStateApp.Areas.Administration.Controllers
{

    [Area("Administration")]
    [Authorize(Roles = "ADMIN")]

    public class HomeController : Controller
    {
        private IAdministrationService _administrationService;
        private readonly IMapper _mapper;

        public HomeController(IAdministrationService administrationService, IMapper mapper)
        {
            _administrationService = administrationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
          var statsDto= await _administrationService.GetAdminStats();
            return View(_mapper.Map<AdminDashboardViewModel>(statsDto));
        }
    }
}
