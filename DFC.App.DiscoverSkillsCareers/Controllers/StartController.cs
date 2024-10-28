using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Constants = DFC.Common.SharedContent.Pkg.Netcore.Constant.ApplicationKeys;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class StartController : BaseController
    {
        private const string ExpiryAppSettings = "Cms:Expiry";
        private readonly IAssessmentService assessmentService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IConfiguration configuration;
        private string status;
        private double expiryInHours = 4;
        private readonly ILogService logService;

        public StartController(
            ISessionService sessionService,
            IAssessmentService assessmentService,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IConfiguration configuration,
            ILogService logService)
            : base(sessionService)
        {
            this.assessmentService = assessmentService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.configuration = configuration;
            this.logService = logService;

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

        public async Task<IActionResult> IndexAsync()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var startViewModel = await GetAssessmentViewModel().ConfigureAwait(false);

            var sharedhtml = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserSharedContent, status, expiryInHours).Result.Html;
            startViewModel.SharedContent = sharedhtml;

            return View(startViewModel);
        }


        private async Task<StartViewModel> GetAssessmentViewModel()
        {
            var getAssessmentResponse = await GetAssessment().ConfigureAwait(false);

            var result = new StartViewModel
            {
                ReferenceCode = SessionHelper.FormatSessionId(getAssessmentResponse.ReferenceCode!),
                AssessmentStarted = getAssessmentResponse.StartedDt.ToString(DateTimeFormat.Standard),
            };

            return result;
        }

        public IActionResult EmailSent()
        {
            logService.LogInformation($"{nameof(this.EmailSent)} generated the model and ready to pass to the view");

            return View();
        }

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            return await assessmentService.GetAssessment().ConfigureAwait(false);
        }
    }
}
