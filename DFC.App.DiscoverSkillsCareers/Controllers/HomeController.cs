using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Constants = DFC.Common.SharedContent.Pkg.Netcore.Constant.ApplicationKeys;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class HomeController : BaseController
    {
        private const string ExpiryAppSettings = "Cms:Expiry";
        private readonly IAssessmentService assessmentService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IConfiguration configuration;
        private string status;
        private double expiryInHours = 4;

        public HomeController(ISessionService sessionService, IAssessmentService assessmentService, ISharedContentRedisInterface sharedContentRedisInterface, IConfiguration configuration)
            : base(sessionService)
        {
            this.assessmentService = assessmentService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.configuration = configuration;

            status = configuration?.GetSection("contentMode:contentMode").Get<string>();

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            if (this.configuration != null)
            {
                string expiryAppString = this.configuration.GetSection(ExpiryAppSettings).Get<string>();
                if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
                {
                    expiryInHours = expiryAppStringParseResult;
                }
            }
        }

        public Task<IActionResult> IndexAsync()
        {
            var responseVm = new HomeIndexResponseViewModel
            {
                SpeakToAnAdviser = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserFooterSharedContent, status, expiryInHours).Result.Html,
            };
            return Task.FromResult<IActionResult>(View(responseVm));
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexRequestViewModel viewModel)
        {
            if (viewModel == null || viewModel.ReferenceCode == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var responseViewModel = new HomeIndexResponseViewModel
                {
                    ReferenceCode = viewModel.ReferenceCode,
                    SpeakToAnAdviser = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserFooterSharedContent, status, expiryInHours).Result.Html,
                };
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
