using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly NotifyOptions notifyOptions;
        private readonly ICommonService commonService;

        public StartController(
            ISessionService sessionService,
            IAssessmentService assessmentService,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IConfiguration configuration,
            ILogService logService,
            ICommonService commonService,
            NotifyOptions notifyOptions)
            : base(sessionService)
        {
            this.assessmentService = assessmentService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.configuration = configuration;
            this.logService = logService;
            this.commonService = commonService;
            status = configuration?.GetSection("contentMode:contentMode").Get<string>();
            this.notifyOptions = notifyOptions;

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

            var sharedhtml = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserFooterSharedContent, status, expiryInHours).Result.Html;
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

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            return await assessmentService.GetAssessment().ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<IActionResult> Index(StartViewModel request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (request.Contact is not null && request.Contact == Core.Enums.AssessmentReturnType.Email)
            {
                SanitiseEmail(request);
            }

            if (!ModelState.IsValid)
            {
                var startViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
                request.SharedContent = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserFooterSharedContent, status, expiryInHours).Result.Html;
                request.AssessmentStarted = startViewModel.AssessmentStarted;
                request.ReferenceCode = startViewModel.ReferenceCode;
                return View(request);
            }

            if (ModelState.IsValid)
            {
                if (request.Contact == Core.Enums.AssessmentReturnType.Reference)
                {
                    if (TempData != null)
                    {
                        const string key = "Telephone";
                        TempData.Remove(key);
                        TempData.Add(key, request.PhoneNumber);
                    }

                    await commonService.SendSms(notifyOptions.ReturnUrl!, request.PhoneNumber).ConfigureAwait(false);

                    return RedirectTo("assessment/referencesent"); // This needs changed once the page is implemented.
                }

                if (request.Contact == Core.Enums.AssessmentReturnType.Email)
                {
                    try
                    {
                        var emailResponse = await commonService.SendEmail(notifyOptions.ReturnUrl!, request.Email).ConfigureAwait(false);

                        if (emailResponse.IsSuccess)
                        {
                            if (TempData != null)
                            {
                                TempData["SentEmail"] = request.Email;
                            }

                            return RedirectTo("start/emailsent");
                        }
                    }
                    catch (Exception exception)
                    {
                        logService.LogError(exception.Message);
                    }
                }
            }

            return View(request);
        }

        private void SanitiseEmail(StartViewModel request)
        {
            request.Email = request.Email?.ToLower();
            ModelState.Clear();

            TryValidateModel(request);
        }
    }
}
