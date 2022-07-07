using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAssessmentService assessmentService;

        public HomeController(ISessionService sessionService, IAssessmentService assessmentService)
            : base(sessionService)
        {
            this.assessmentService = assessmentService;
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
                var responseViewModel = new HomeIndexResponseViewModel { ReferenceCode = viewModel.ReferenceCode };

                return View(responseViewModel);
            }

            var reloadResponse = await assessmentService.ReloadUsingReferenceCode(viewModel.ReferenceCode).ConfigureAwait(false);
            if (reloadResponse)
            {
                return RedirectTo("assessment/return");
            }
            else
            {
                ModelState.AddModelError("ReferenceCode", "The reference could not be found");
                var responseViewModel = new HomeIndexResponseViewModel { ReferenceCode = viewModel.ReferenceCode };
                ViewData["Title"] = "Error";
                return View(responseViewModel);
            }
        }
    }
}
