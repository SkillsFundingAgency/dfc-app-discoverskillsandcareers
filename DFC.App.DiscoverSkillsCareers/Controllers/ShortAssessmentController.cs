using AutoMapper;
using Dfc.Session;
using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Core;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Assessment;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ShortAssessmentController : BaseController
    {
        private const string FailedAnswerErrorMessageKey = "Answer";
        private const string FailedAnswerErrorMessage = "Failed to record answer";

        private readonly IMapper mapper;
        private readonly IAssessmentService<ShortAssessment> shortAssesmentService;

        public ShortAssessmentController(IMapper mapper, ISessionClient sessionService, IAssessmentService<ShortAssessment> shortAssesmentService)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.shortAssesmentService = shortAssesmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int questionNumber)
        {
            if (!await HasSessionIdAsync().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            var questionResponse = await shortAssesmentService.GetQuestionAsync(questionNumber).ConfigureAwait(false);
            if (questionResponse is null || questionNumber > questionResponse.MaxQuestionsCount)
            {
                return BadRequest();
            }

            if (questionResponse.IsComplete)
            {
                //TODO: Convert this if possible to Redirect to action if not move this literal to const
                return RedirectTo("results");
            }

            if (questionNumber > questionResponse.QuestionNumber)
            {
                //TODO: Convert this if possible to Redirect to action if not move this literal to const
                return RedirectToAction(nameof(Index), new { questionNumber = questionResponse.QuestionNumber });
            }

            var responseViewModel = mapper.Map<QuestionGetResponseViewModel>(questionResponse);
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionPostRequestViewModel requestViewModel)
        {
            if (requestViewModel == null)
            {
                return BadRequest();
            }

            if (!await HasSessionIdAsync().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            if (!ModelState.IsValid)
            {
                var question = await shortAssesmentService.GetQuestionAsync(requestViewModel.QuestionNumber).ConfigureAwait(false);
                if (question is null)
                {
                    return BadRequest();
                }

                var questionView = mapper.Map<QuestionGetResponseViewModel>(question);
                return View(questionView);
            }

            var answerResponse = await shortAssesmentService.AnswerQuestionAsync(requestViewModel.QuestionNumber, requestViewModel.Answer).ConfigureAwait(false);
            if (answerResponse.IsSuccess)
            {
                if (answerResponse.IsComplete)
                {
                    return RedirectTo("assessment/complete");
                    //TODO: Can we not use this? RedirectToAction(nameof(Complete));
                }
                else
                {
                    //TODO: Convert this if possible to Redirect to action if not move this literal to const
                    return RedirectTo($"assessment/{requestViewModel.AssessmentType}/{answerResponse.NextQuestionNumber}");
                }
            }
            else
            {
                ModelState.AddModelError(FailedAnswerErrorMessageKey, FailedAnswerErrorMessage);
                return View(answerResponse.Question);
            }
        }

        [HttpPost]
        public async Task<IActionResult> New(DfcUserSession userSession)
        {
            await shortAssesmentService.CreateAssessmentAsync(userSession).ConfigureAwait(false);

            //TODO: Convert this if possible to Redirect to action if not move this literal to const
            return RedirectTo($"assessment/{AssessmentTypeName.ShortAssessment}/1");
        }

        public IActionResult Complete() => View();

        public async Task<IActionResult> Return() => NavigateTo(await shortAssesmentService.GetAssessmentAsync().ConfigureAwait(false));

        [HttpGet]
        public IActionResult Save() => View();

        [HttpPost]
        public IActionResult Save(AssessmentSaveRequestViewModel viewModel)
        {
            if (viewModel is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            switch (viewModel.AssessmentReturnTypeId)
            {
                case AssessmentReturnType.Email:
                    //TODO: Convert this if possible to Redirect to action if not move this literal to const
                    return RedirectTo("assessment/email");

                case AssessmentReturnType.Reference:
                    //TODO: Convert this if possible to Redirect to action if not move this literal to const
                    return RedirectTo("assessment/reference");

                default:
                    return View(viewModel);
            }
        }

        [HttpGet]
        public IActionResult Email() => View();

        [HttpPost]
        public async Task<IActionResult> Email(AssessmentEmailPostRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await apiService.SendEmail($"https://{Request.Host.Value}", request.Email, "1").ConfigureAwait(false);

                return RedirectTo("assessment/emailsent");
            }

            return View(request);
        }

        public IActionResult EmailSent()
        {
            return View();
        }

        public async Task<IActionResult> Reference()
        {
            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
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

                return RedirectTo("assessment/referencesent");
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            return View(responseViewModel);
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }

        //TODO: May not need this?
        private static string GetAssessmentTypeName(string value)
        {
            var result = string.Empty;
            if (Enum.TryParse<Assessment>(value, true, out var assessmentItemType))
            {
                result = assessmentItemType.ToString().ToLower();
            }

            return result;
        }

        private async Task<AssessmentReferenceGetResponse> GetAssessmentViewModel()
        {
            var getAssessmentResponse = await shortAssesmentService.GetAssessment().ConfigureAwait(false); ;

            var result = new AssessmentReferenceGetResponse();
            result.ReferenceCode = getAssessmentResponse.ReferenceCode;
            result.AssessmentStarted = getAssessmentResponse.StartedDt.ToString("d MMMM yyyy");

            return result;
        }

        private IActionResult NavigateTo(GetAssessmentResponse assessment)
        {
            if (assessment == null)
            {
                return BadRequest();
            }

            if (assessment.IsComplete)
            {
                return RedirectTo("results");
            }

            return RedirectTo($"assessment/{assessment.QuestionSetName}/{assessment.CurrentQuestionNumber}");
        }
    }
}