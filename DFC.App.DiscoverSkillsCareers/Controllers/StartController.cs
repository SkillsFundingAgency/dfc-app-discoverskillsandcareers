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
using System.Web;
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
            startViewModel.DysacAction = Core.Enums.DysacAction.Start;

            TempData["sharedcontent"] = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserFooterSharedContent, status, expiryInHours).Result.Html;

            return View(startViewModel);
        }

        public async Task<IActionResult> Reference()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var startViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            startViewModel.DysacAction = Core.Enums.DysacAction.Return;

            return View(startViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(StartViewModel request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            TempData["sharedcontent"] = sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(Constants.SpeakToAnAdviserFooterSharedContent, status, expiryInHours).Result.Html;

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            if (request.Contact is not null && request.Contact == Core.Enums.AssessmentReturnType.Email)
            {
                SanitiseEmail(request);
            }

            return request.Contact == Core.Enums.AssessmentReturnType.Reference ? await SendSms(request).ConfigureAwait(false) :
                await SendEmail(request).ConfigureAwait(false);
        }

        public IActionResult EmailSent()
        {
            logService.LogInformation($"{nameof(this.EmailSent)} generated the model and ready to pass to the view");

            return View();
        }

        public IActionResult ReferenceSent()
        {
            logService.LogInformation($"{nameof(this.ReferenceSent)} generated the model and ready to pass to the view");

            return View();
        }

        private async Task<IActionResult> SendEmail(StartViewModel request)
        {
            try
            {
                var emailResponse = await commonService.SendEmail(notifyOptions.ReturnUrl!, request.Email).ConfigureAwait(false);

                if (emailResponse.IsSuccess)
                {
                    if (TempData != null)
                    {
                        TempData["SentEmail"] = request.Email;
                        if (request.DysacAction == Core.Enums.DysacAction.Start)
                        {
                            TempData["DysacAction"] = "Start";
                        }
                        else
                        {
                            TempData["DysacAction"] = "Return";
                        }
                    }

                    return RedirectTo("start/emailsent");
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.Message);
            }

            ModelState.AddModelError("Email", "There was a problem sending email");
            return View(request);
        }

        private async Task<IActionResult> SendSms(StartViewModel request)
        {
            try
            {
                if (TempData != null)
            {
                const string key = "Telephone";
                TempData.Remove(key);
                TempData.Add(key, request.PhoneNumber);
                if (request.DysacAction == Core.Enums.DysacAction.Start)
                {
                   TempData["DysacAction"] = "Start";
                }
                else
                {
                   TempData["DysacAction"] = "Return";
                }
             }

                await commonService.SendSms(notifyOptions.ReturnUrl!, request.PhoneNumber).ConfigureAwait(false);

                return RedirectTo("start/referencesent"); // This needs changed once the page is implemented.
            }
            catch (Exception exception)
            {
                logService.LogError(exception.Message);
            }

            ModelState.AddModelError("PhoneNumber", "There was a problem sending a text message");
            return View(request);
        }

        private void SanitiseEmail(StartViewModel request)
        {
            request.Email = request.Email?.ToLower();
            ModelState.Clear();

            TryValidateModel(request);
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
    }
}
