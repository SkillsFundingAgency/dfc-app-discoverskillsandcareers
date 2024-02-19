using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class AssessmentController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;
        private readonly NotifyOptions notifyOptions;

        public AssessmentController(
            ILogService logService,
            IMapper mapper,
            IAssessmentService assessmentService,
            ISessionService sessionService,
            NotifyOptions notifyOptions)
                : base(sessionService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.assessmentService = assessmentService;
            this.notifyOptions = notifyOptions;
        }

        [HttpGet]
        public async Task<IActionResult> Index(QuestionGetRequestViewModel requestViewModel)
        {
            logService.LogInformation($"{nameof(this.Index)} started");

            if (requestViewModel == null)
            {
                logService.LogInformation($"BadRequest {requestViewModel}");
                return BadRequest();
            }

            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                logService.LogInformation($"RedirectToRoot hasSessionId {hasSessionId}");
                return RedirectToRoot();
            }

            var questionResponse = await GetQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber).ConfigureAwait(false);
            if (questionResponse == null)
            {
                logService.LogInformation($"questionResponse {questionResponse} BadRequest ");
                return BadRequest();
            }

            if (requestViewModel.QuestionNumber > questionResponse.MaxQuestionsCount)
            {
                logService.LogInformation($"questionResponse {questionResponse} requestViewModel.QuestionNumber {requestViewModel.QuestionNumber} BadRequest ");
                return BadRequest();
            }

            if (questionResponse.IsComplete)
            {
                logService.LogInformation($"questionResponse {questionResponse} IsComplete redirecting to resulsts ");

                return RedirectTo("results");
            }

            var assessment = await assessmentService.GetAssessment().ConfigureAwait(false);

            if (requestViewModel.QuestionNumber > assessment.CurrentQuestionNumber)
            {
                logService.LogInformation($"QuestionNumber>CurrentQuestionNumber assessment/{requestViewModel.AssessmentType}/{assessment.CurrentQuestionNumber}");

                return RedirectTo($"assessment/{requestViewModel.AssessmentType}/{assessment.CurrentQuestionNumber}");
            }

            var hasGoneBackOneOrMoreQuestions = requestViewModel.QuestionNumber + 1 <= assessment.CurrentQuestionNumber;

            if (hasGoneBackOneOrMoreQuestions)
            {
                logService.LogInformation($"hasGoneBackOneOrMoreQuestions {hasGoneBackOneOrMoreQuestions}");

                assessment.CurrentQuestionNumber = requestViewModel.QuestionNumber;

                var completed = (int)((assessment.CurrentQuestionNumber - 1) / (decimal)assessment.MaxQuestionsCount * 100M);
                assessment.PercentComplete = completed;

                logService.LogInformation($"assessment.PercentComplete {assessment.PercentComplete}");

                questionResponse.PercentComplete = assessment.PercentComplete;
            }

            var responseViewModel = mapper.Map<QuestionGetResponseViewModel>(questionResponse);
            logService.LogInformation($"{nameof(this.Index)} generated the model and ready to pass to the view");

            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionPostRequestViewModel requestViewModel)
        {
            if (requestViewModel == null)
            {
                logService.LogInformation($"Index requestviewmodel is null BadRequest");
                return BadRequest();
            }

            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                logService.LogInformation($"hasSessionId {hasSessionId} RedirectToRoot");
                return RedirectToRoot();
            }

            var question = await GetQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber).ConfigureAwait(false);
            if (question == null)
            {
                logService.LogInformation($"question {hasSessionId} RedirectToRoot");
                return BadRequest();
            }

            var result = mapper.Map<QuestionGetResponseViewModel>(question);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var answerResponse = await assessmentService.AnswerQuestion(
                requestViewModel.AssessmentType,
                requestViewModel.QuestionNumber,
                requestViewModel.QuestionNumber,
                requestViewModel.Answer).ConfigureAwait(false);

            if (answerResponse.IsSuccess)
            {
                if (answerResponse.IsComplete)
                {
                    return RedirectTo("assessment/complete");
                }

                var assessmentTypeName = GetAssessmentTypeName(requestViewModel.AssessmentType);
                return RedirectTo($"assessment/{assessmentTypeName}/{answerResponse.NextQuestionNumber}");
            }

            ModelState.AddModelError("Answer", "Failed to record answer");
            return View(result);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> New(string assessmentType)
        {
            logService.LogInformation($"{nameof(New)} started");

            if (string.IsNullOrEmpty(assessmentType))
            {
                return NoContent();
            }

            await assessmentService.NewSession(assessmentType).ConfigureAwait(false);
            logService.LogInformation($"{nameof(New)} generated the model and ready to pass to the view");

            return RedirectTo($"assessment/{GetAssessmentTypeName(assessmentType)}/1");
        }

        public IActionResult Complete()
        {
            logService.LogInformation($"{nameof(Complete)} generated the model and ready to pass to the view");
            return View();
        }

        public async Task<IActionResult> Return()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var assessment = await GetAssessment().ConfigureAwait(false);
            logService.LogInformation($"{nameof(Return)} generated the model and ready to pass to the view");

            return NavigateTo(assessment);
        }

        public async Task<IActionResult> Save()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            logService.LogInformation($"{nameof(this.Save)} generated the model and ready to pass to the view");
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

            if (viewModel.AssessmentReturnTypeId == AssessmentReturnType.Reference)
            {
                return RedirectTo("assessment/reference");
            }

            return View();
        }

        public async Task<IActionResult> Email()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var viewResponse = new AssessmentEmailPostRequest();

            logService.LogInformation($"{nameof(this.Email)} generated the model and ready to pass to the view");

            return View(viewResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Email(AssessmentEmailPostRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            SanitiseEmail(request);

            if (!ModelState.IsValid)
            {
                return View(new AssessmentEmailPostRequest { Email = request.Email });
            }

            try
            {
                var emailResponse = await assessmentService.SendEmail(notifyOptions.ReturnUrl!, request.Email).ConfigureAwait(false);

                if (emailResponse.IsSuccess)
                {
                    if (TempData != null)
                    {
                        TempData["SentEmail"] = request.Email;
                    }

                    return RedirectTo("assessment/emailsent");
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.Message);
            }

            ModelState.AddModelError("Email", "There was a problem sending email");
            return View(new AssessmentEmailPostRequest { Email = request.Email });
        }

        public IActionResult EmailSent()
        {
            logService.LogInformation($"{nameof(this.EmailSent)} generated the model and ready to pass to the view");

            return View();
        }

        public async Task<IActionResult> Reference()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);

            logService.LogInformation($"{nameof(this.Reference)} generated the model and ready to pass to the view");
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
                    const string key = "Telephone";
                    TempData.Remove(key);
                    TempData.Add(key, request.Telephone);
                }

                await assessmentService.SendSms(notifyOptions.ReturnUrl!, request.Telephone).ConfigureAwait(false);

                return RedirectTo("assessment/referencesent");
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            logService.LogInformation($"{nameof(this.Reference)} generated the model and ready to pass to the view");

            return View(responseViewModel);
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }

        [Route("head/reload")]
        public async Task<IActionResult> Reload(string sessionId)
        {
            var reloadResponseSuccess = await assessmentService.ReloadUsingSessionId(sessionId).ConfigureAwait(false);

            if (!reloadResponseSuccess)
            {
                return RedirectToRoot();
            }

            logService.LogInformation($"{nameof(this.Reload)} generated the model and ready to pass to the view");
            return RedirectTo("assessment/return");
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

        private void SanitiseEmail(AssessmentEmailPostRequest request)
        {
            request.Email = request.Email?.ToLower();
            ModelState.Clear();

            TryValidateModel(request);
        }

        private async Task<AssessmentReferenceGetResponse> GetAssessmentViewModel()
        {
            var getAssessmentResponse = await GetAssessment().ConfigureAwait(false);

            var result = new AssessmentReferenceGetResponse
            {
                ReferenceCode = SessionHelper.FormatSessionId(getAssessmentResponse.ReferenceCode!),
                AssessmentStarted = getAssessmentResponse.StartedDt.ToString(DateTimeFormat.Standard),
            };

            return result;
        }

        private async Task<GetQuestionResponse> GetQuestion(string assessmentType, int questionNumber)
        {
            var result = await assessmentService.GetQuestion(assessmentType, questionNumber).ConfigureAwait(false);
            
            return result;
        }

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            return await assessmentService.GetAssessment().ConfigureAwait(false);
        }

        private IActionResult NavigateTo(GetAssessmentResponse assessment)
        {
            if (assessment == null)
            {
                return BadRequest();
            }

            if (assessment.IsFilterAssessment)
            {
                if (assessment.IsFilterComplete)
                {
                    if (!string.IsNullOrEmpty(assessment.CurrentFilterAssessmentCode))
                    {
                        return RedirectTo($"results/roles/{assessment.CurrentFilterAssessmentCode}");
                    }

                    return RedirectTo($"results/roles/{assessment.JobCategorySafeUrl}");
                }

                return RedirectTo(
                    $"{AssessmentItemType.Short.ToString().ToLower()}/filterquestions/{assessment.CurrentFilterAssessmentCode}/{assessment.CurrentQuestionNumber}");
            }

            return RedirectTo(assessment.IsComplete ?
                "results" :
                $"assessment/short/{assessment.CurrentQuestionNumber}");
        }
    }
}
