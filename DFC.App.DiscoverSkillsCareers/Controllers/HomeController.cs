using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IApiService apiService;

        public HomeController(ISessionService sessionService, IApiService apiService)
            : base(sessionService)
        {
            this.apiService = apiService;
        }

        public IActionResult Index()
        {
            var responseVm = new HomeIndexResponseViewModel();

            return View(responseVm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var repsonseViewModel = new HomeIndexResponseViewModel() { ReferenceCode = viewModel.ReferenceCode };
                return View(repsonseViewModel);
            }

            await apiService.Reload(viewModel.ReferenceCode).ConfigureAwait(false);

            return RedirectTo("assessment/return");
        }
    }
}
