using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class AssessmentController : BaseController<AssessmentController>
    {
        private readonly IMapper mapper;
        private readonly IAssessmentService apiService;
        private readonly NotifyOptions notifyOptions;

        public AssessmentController(ILogger<AssessmentController> logger, IMapper mapper, IAssessmentService apiService,
            ISessionStateService<SessionDataModel> sessionStateService, NotifyOptions notifyOptions)
            : base(logger, sessionStateService)
        {
            this.mapper = mapper;
            this.apiService = apiService;
            this.notifyOptions = notifyOptions;
        }

        [HttpGet]
        public async Task<IActionResult> Index(QuestionGetRequestViewModel requestViewModel)
        {
            Logger.LogInformation($"{nameof(this.Index)} started");

            if (requestViewModel == null)
            {
                return BadRequest();
            }

            var hasSessionId = HasSessionId();
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var questionResponse = await GetQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber).ConfigureAwait(false);

            if (questionResponse == null)
            {
                return BadRequest();
            }

            var assessment = await apiService.GetAssessment(Request.CompositeSessionId()).ConfigureAwait(false);
            if (assessment == null)
            {
                return BadRequest();
            }

            if (requestViewModel.QuestionNumber > questionResponse.MaxQuestionsCount)
            {
                return BadRequest();
            }

            if (questionResponse.IsComplete)
            {
                return RedirectTo("results");
            }

            if (requestViewModel.QuestionNumber > assessment.CurrentQuestionNumber)
            {
                return RedirectTo($"assessment/{requestViewModel.AssessmentType}/{assessment.CurrentQuestionNumber}");
            }

            var responseViewModel = mapper.Map<QuestionGetResponseViewModel>(questionResponse);
            Logger.LogInformation($"{nameof(this.Index)} generated the model and ready to pass to the view");

            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionPostRequestViewModel requestViewModel)
        {
            if (requestViewModel == null)
            {
                return BadRequest();
            }

            var hasSessionId = HasSessionId();
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var question = await GetQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber).ConfigureAwait(false);
            if (question == null)
            {
                return BadRequest();
            }

            var result = mapper.Map<QuestionGetResponseViewModel>(question);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var answerResponse = await apiService.AnswerQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber, requestViewModel.QuestionNumber, requestViewModel.Answer, Request.CompositeSessionId()).ConfigureAwait(false);

            if (answerResponse.IsSuccess)
            {
                if (answerResponse.IsComplete)
                {
                    return RedirectTo("assessment/complete");
                }
                else
                {
                    var assessmentTypeName = GetAssessmentTypeName(requestViewModel.AssessmentType);
                    return RedirectTo($"assessment/{assessmentTypeName}/{answerResponse.NextQuestionNumber}");
                }
            }
            else
            {
                ModelState.AddModelError("Answer", "Failed to record answer");
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> New(string assessmentType)
        {
            Logger.LogInformation($"{nameof(this.New)} started");

            if (string.IsNullOrEmpty(assessmentType))
            {
                return BadRequest();
            }

            await apiService.NewSession(assessmentType, Request.CompositeSessionId()).ConfigureAwait(false);

            Logger.LogInformation($"{nameof(this.New)} generated the model and ready to pass to the view");

            return RedirectTo($"assessment/{GetAssessmentTypeName(assessmentType)}/1");
        }

        public IActionResult Complete()
        {
            Logger.LogInformation($"{nameof(this.Complete)} generated the model and ready to pass to the view");

            return View();
        }

        public async Task<IActionResult> Return()
        {
            var hasSessionId = HasSessionId();
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var assessment = await GetAssessment().ConfigureAwait(false);

            Logger.LogInformation($"{nameof(this.Return)} generated the model and ready to pass to the view");

            return NavigateTo(assessment);
        }

        public async Task<IActionResult> Save()
        {
            var hasSessionId = HasSessionId();
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            Logger.LogInformation($"{nameof(this.Save)} generated the model and ready to pass to the view");
            return View();
        }

        [HttpPost]
        public IActionResult Save(AssessmentSaveRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.AssessmentReturnTypeId == AssessmentReturnType.Email)
            {
                return RedirectTo("assessment/email");
            }
            else if (viewModel.AssessmentReturnTypeId == AssessmentReturnType.Reference)
            {
                return RedirectTo("assessment/reference");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Email()
        {
            var hasSessionId = HasSessionId();
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var viewReponse = new AssessmentEmailPostRequest();

            Logger.LogInformation($"{nameof(this.Email)} generated the model and ready to pass to the view");

            return View(viewReponse);
        }

        [HttpPost]
        public async Task<IActionResult> Email(AssessmentEmailPostRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var viewReponse = new AssessmentEmailPostRequest() { Email = request.Email };
                return View(viewReponse);
            }

            var emailResponse = await apiService.SendEmail(notifyOptions.ReturnUrl, request.Email, Request.CompositeSessionId()).ConfigureAwait(false);
            if (emailResponse.IsSuccess)
            {
                if (TempData != null)
                {
                    TempData["SentEmail"] = request.Email;
                }

                return RedirectTo("assessment/emailsent");
            }
            else
            {
                ModelState.AddModelError("Email", "There was a problem sending email");
                var viewReponse = new AssessmentEmailPostRequest() { Email = request.Email };
                return View(viewReponse);
            }
        }

        public IActionResult EmailSent()
        {
            Logger.LogInformation($"{nameof(this.EmailSent)} generated the model and ready to pass to the view");

            return View();
        }

        public async Task<IActionResult> Reference()
        {
            var hasSessionId = HasSessionId();
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);

            Logger.LogInformation($"{nameof(this.Reference)} generated the model and ready to pass to the view");
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reference(AssessmentReferencePostRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                if (TempData != null)
                {
                    var key = "Telephone";
                    TempData.Remove(key);
                    TempData.Add(key, request.Telephone);
                }

                await apiService.SendSms(GetDomainUrl(), request.Telephone, Request.CompositeSessionId()).ConfigureAwait(false);

                return RedirectTo("assessment/referencesent");
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            Logger.LogInformation($"{nameof(this.Reference)} generated the model and ready to pass to the view");

            return View(responseViewModel);
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }

        [Route("head/reload")]
        public async Task<IActionResult> Reload(string sessionId)
        {
            var reloadResponseSuccess = await apiService.ReloadUsingSessionId(Request.CompositeSessionId()).ConfigureAwait(false);
            if (reloadResponseSuccess)
            {
                Logger.LogInformation($"{nameof(this.Reload)} generated the model and ready to pass to the view");

                return RedirectTo("assessment/return");
            }
            else
            {
                return RedirectToRoot();
            }
        }

        private static string GetAssessmentTypeName(string value)
        {
            var result = string.Empty;
            if (Enum.TryParse<AssessmentItemType>(value, true, out var assessmentItemType))
            {
                result = assessmentItemType.ToString().ToLower();
            }

            return result;
        }

        private async Task<AssessmentReferenceGetResponse> GetAssessmentViewModel()
        {
            var getAssessmentResponse = await GetAssessment().ConfigureAwait(false);

            var result = new AssessmentReferenceGetResponse
            {
                ReferenceCode = getAssessmentResponse.ReferenceCode,
                AssessmentStarted = getAssessmentResponse.StartedDt.ToString(DateTimeFormat.Standard),
            };

            return result;
        }

        private async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            var question = await apiService.GetQuestion(assessmentType, questionNumber, GetDysacIdAsync()).ConfigureAwait(false);

            return question;
        }

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            var getAssessmentResponse = await apiService.GetAssessment(Request.CompositeSessionId()).ConfigureAwait(false);
            return getAssessmentResponse;
        }

        private IActionResult NavigateTo(GetAssessmentResponse assessment)
        {
            if (assessment == null)
            {
                return BadRequest();
            }

            if (assessment.IsFilterAssessment)
            {
                if (assessment.IsComplete)
                {
                    return RedirectTo($"results/roles/{assessment.JobCategorySafeUrl}");
                }
                else
                {
                    return RedirectTo($"{AssessmentItemType.Short.ToString().ToLower()}/filterquestions/{assessment.JobCategorySafeUrl}/{assessment.CurrentQuestionNumber}");
                }
            }
            else
            {
                if (assessment.IsComplete)
                {
                    return RedirectTo("results");
                }
                else
                {
                    return RedirectTo($"assessment/{assessment.QuestionSetName}/{assessment.CurrentQuestionNumber}");
                }
            }
        }

        private string GetDomainUrl()
        {
            return $"https://{Request.Host.Value}/{RouteName.Prefix}/assessment";
        }
    }
}