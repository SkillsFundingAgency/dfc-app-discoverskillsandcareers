using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
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

        public AssessmentController(ILogService logService, IMapper mapper, IAssessmentService apiService, ISessionService sessionService, NotifyOptions notifyOptions)
            : base(sessionService)
        {
            this.logService = logService;
            this.mapper = mapper;
            this.assessmentService = apiService;
            this.notifyOptions = notifyOptions;
        }

        [HttpGet]
        public async Task<IActionResult> Index(QuestionGetRequestViewModel requestViewModel)
        {
            this.logService.LogInformation($"{nameof(this.Index)} started");

            if (requestViewModel == null)
            {
                return BadRequest();
            }

            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var questionResponse = await GetQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber).ConfigureAwait(false);

            if (questionResponse == null)
            {
                return BadRequest();
            }

            var assessment = await assessmentService.GetAssessment().ConfigureAwait(false);
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

            var hasGoneBackOneOrMoreQuestions = (requestViewModel.QuestionNumber + 1) <= assessment.CurrentQuestionNumber;

            if (hasGoneBackOneOrMoreQuestions)
            {
                assessment.CurrentQuestionNumber = requestViewModel.QuestionNumber;

                var completed = (int)(((assessment.CurrentQuestionNumber - 1) / (decimal)assessment.MaxQuestionsCount) * 100M);
                assessment.PercentComplete = completed;
                questionResponse.PercentComplete = assessment.PercentComplete;

                await assessmentService.UpdateQuestionNumber(assessment.CurrentQuestionNumber).ConfigureAwait(false);
            }

            var responseViewModel = mapper.Map<QuestionGetResponseViewModel>(questionResponse);
            this.logService.LogInformation($"{nameof(this.Index)} generated the model and ready to pass to the view");

            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionPostRequestViewModel requestViewModel)
        {
            if (requestViewModel == null)
            {
                return BadRequest();
            }

            var hasSessionId = await HasSessionId().ConfigureAwait(false);
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

            var answerResponse = await assessmentService.AnswerQuestion(requestViewModel.AssessmentType, requestViewModel.QuestionNumber, requestViewModel.QuestionNumber, requestViewModel.Answer).ConfigureAwait(false);

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
            this.logService.LogInformation($"{nameof(this.New)} started");

            if (string.IsNullOrEmpty(assessmentType))
            {
                return BadRequest();
            }

            await assessmentService.NewSession(assessmentType).ConfigureAwait(false);

            this.logService.LogInformation($"{nameof(this.New)} generated the model and ready to pass to the view");

            return RedirectTo($"assessment/{GetAssessmentTypeName(assessmentType)}/1");
        }

        public IActionResult Complete()
        {
            this.logService.LogInformation($"{nameof(this.Complete)} generated the model and ready to pass to the view");

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

            this.logService.LogInformation($"{nameof(this.Return)} generated the model and ready to pass to the view");

            return NavigateTo(assessment);
        }

        public async Task<IActionResult> Save()
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            this.logService.LogInformation($"{nameof(this.Save)} generated the model and ready to pass to the view");
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

            var viewReponse = new AssessmentEmailPostRequest();

            this.logService.LogInformation($"{nameof(this.Email)} generated the model and ready to pass to the view");

            return View(viewReponse);
        }

        private void SanitiseEmail(AssessmentEmailPostRequest request)
        {
            request.Email = request.Email?.ToLower();
            ModelState.Clear();

            TryValidateModel(request);
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
                var emailResponse = await assessmentService.SendEmail(notifyOptions.ReturnUrl, request.Email).ConfigureAwait(false);

                if (emailResponse.IsSuccess)
                {
                    if (TempData != null)
                    {
                        TempData["SentEmail"] = request.Email;
                    }

                    return RedirectTo("assessment/emailsent");
                }
            }
            catch (Exception ex)
            {
                logService.LogError(ex.Message);
            }

            ModelState.AddModelError("Email", "There was a problem sending email");
            return View(new AssessmentEmailPostRequest { Email = request.Email });
        }

        public IActionResult EmailSent()
        {
            this.logService.LogInformation($"{nameof(this.EmailSent)} generated the model and ready to pass to the view");

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

            this.logService.LogInformation($"{nameof(this.Reference)} generated the model and ready to pass to the view");
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

                await assessmentService.SendSms(notifyOptions.ReturnUrl, request.Telephone).ConfigureAwait(false);

                return RedirectTo("assessment/referencesent");
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            this.logService.LogInformation($"{nameof(this.Reference)} generated the model and ready to pass to the view");

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
            if (reloadResponseSuccess)
            {
                this.logService.LogInformation($"{nameof(this.Reload)} generated the model and ready to pass to the view");

                return RedirectTo("assessment/return");
            }

            return RedirectToRoot();
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
            return await assessmentService.GetQuestion(assessmentType, questionNumber).ConfigureAwait(false);
        }

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            var getAssessmentResponse = await assessmentService.GetAssessment().ConfigureAwait(false);
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
                if (assessment.IsFilterComplete)
                {
                    return RedirectTo($"results/roles/{assessment.JobCategorySafeUrl}");
                }

                return RedirectTo($"{AssessmentItemType.Short.ToString().ToLower()}/filterquestions/{assessment.CurrentFilterAssessmentCode}/{assessment.CurrentQuestionNumber}");
            }

            if (assessment.IsComplete)
            {
                return RedirectTo("results");
            }

            return RedirectTo($"assessment/short/{assessment.CurrentQuestionNumber}");
        }
    }
}