using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class ShortAssessmentController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAssesmentService<ShortAssessment> shortAssesmentService;

        public ShortAssessmentController(IMapper mapper, ISessionService sessionService, IAssesmentService<ShortAssessment> shortAssesmentService)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.shortAssesmentService = shortAssesmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(QuestionGetRequestViewModel requestViewModel)
        {
            if (requestViewModel == null)
            {
                return BadRequest();
            }

            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            var questionResponse = await shortAssesmentService.GetQuestion(requestViewModel.QuestionNumber).ConfigureAwait(false);
            if (questionResponse == null)
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

            if (requestViewModel.QuestionNumber > questionResponse.QuestionNumber)
            {
                return RedirectTo($"assessment/{requestViewModel.AssessmentType}/{questionResponse.QuestionNumber}");
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

            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            var question = await shortAssesmentService.GetQuestion(requestViewModel.QuestionNumber).ConfigureAwait(false);
            if (question is null)
            {
                return BadRequest();
            }

            var result = mapper.Map<QuestionGetResponseViewModel>(question);
            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var answerResponse = await shortAssesmentService.AnswerQuestion(requestViewModel.QuestionNumber, requestViewModel.Answer).ConfigureAwait(false);
            if (answerResponse.IsSuccess)
            {
                if (answerResponse.IsComplete)
                {
                    return RedirectTo("assessment/complete");
                }
                else
                {
                    return RedirectTo($"assessment/{requestViewModel.AssessmentType}/{answerResponse.NextQuestionNumber}");
                }
            }
            else
            {
                ModelState.AddModelError("Answer", "Failed to record answer");
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> New(AssessmentItemType assessmentType)
        {
            switch (assessmentType)
            {
                case AssessmentItemType.Short:
                    break;
                default:
                    break;
            }
            await shortAssesmentService.CreateAssesment(null).ConfigureAwait(false);

            return RedirectTo($"assessment/{GetAssessmentTypeName(assessmentType)}/1");
        }

        public IActionResult Complete()
        {
            return View();
        }

        public async Task<IActionResult> Return()
        {
            var assessment = await GetAssessment().ConfigureAwait(false);
            return NavigateTo(assessment);
        }

        public IActionResult Save()
        {
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

        public IActionResult Email()
        {
            return View();
        }

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
            if (Enum.TryParse<AssessmentItemType>(value, true, out var assessmentItemType))
            {
                result = assessmentItemType.ToString().ToLower();
            }

            return result;
        }

        private async Task<AssessmentReferenceGetResponse> GetAssessmentViewModel()
        {
            var getAssessmentResponse = await GetAssessment().ConfigureAwait(false);

            var result = new AssessmentReferenceGetResponse();
            result.ReferenceCode = getAssessmentResponse.ReferenceCode;
            result.AssessmentStarted = getAssessmentResponse.StartedDt.ToString("d MMMM yyyy");

            return result;
        }

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            var getAssessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            return getAssessmentResponse;
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