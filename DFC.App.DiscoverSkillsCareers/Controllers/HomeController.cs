using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAssessmentService apiService;

        public HomeController(ISessionService sessionService, IAssessmentService apiService)
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
                var responseViewModel = new HomeIndexResponseViewModel() { ReferenceCode = viewModel.ReferenceCode };
                return View(responseViewModel);
            }

            await apiService.ReloadUsingReferenceCode(viewModel.ReferenceCode).ConfigureAwait(false);

            return RedirectTo("assessment/return");
        }
    }
}
