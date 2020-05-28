using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class TestController : BaseController
    {
        private readonly IAssessmentService apiService;
        private readonly ILogService logService;

        public TestController(ILogService logService, ISessionService sessionService, IAssessmentService apiService)
            : base(sessionService)
        {
            this.logService = logService;
            this.apiService = apiService;
        }

        [HttpGet]
        public IActionResult LoadSession()
        {
            var responseVm = new TestLoadSessionResponseViewModel();
            return View(responseVm);
        }

        [HttpPost]
        public async Task<IActionResult> LoadSession(TestLoadSessionRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var responseViewModel = new TestLoadSessionResponseViewModel() { SessionId = viewModel.SessionId };
                return View(responseViewModel);
            }

            var result = await apiService.ReloadUsingSessionId(viewModel.SessionId).ConfigureAwait(false);

            if (result)
            {
                return RedirectTo("assessment/short/1");
            }
            else
            {
                ModelState.AddModelError("SessionId", "SessionId is not valid");
                var responseViewModel = new TestLoadSessionResponseViewModel() { SessionId = viewModel.SessionId };
                this.logService.LogInformation($"{nameof(this.LoadSession)} generated the model and ready to pass to the view");
                return View(responseViewModel);
            }
        }
    }
}
