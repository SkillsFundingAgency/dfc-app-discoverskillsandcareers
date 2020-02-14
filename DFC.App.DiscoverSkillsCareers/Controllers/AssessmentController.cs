using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class AssessmentController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IApiService apiService;

        public AssessmentController(IMapper mapper, ISessionService sessionService, IApiService apiService)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.apiService = apiService;
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
                return Redirect("/");
            }

            var question = await GetQuestion(requestViewModel.QuestionSetName, requestViewModel.QuestionNumber).ConfigureAwait(false);

            if (question == null)
            {
                return BadRequest();
            }

            var getAssessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);

            if (requestViewModel.QuestionNumber > getAssessmentResponse.MaxQuestionsCount)
            {
                return BadRequest();
            }

            if (getAssessmentResponse.IsComplete)
            {
                return RedirectTo("results");
            }

            if (requestViewModel.QuestionNumber > getAssessmentResponse.QuestionNumber)
            {
                return RedirectTo($"assessment/{requestViewModel.QuestionSetName}/{getAssessmentResponse.QuestionNumber}");
            }

            var responseViewModel = mapper.Map<QuestionGetResponseViewModel>(question);
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionPostRequestViewModel requestViewModel)
        {
            if (requestViewModel == null)
            {
                return BadRequest();
            }

            var question = await GetQuestion(requestViewModel.QuestionSetName, requestViewModel.QuestionNumber).ConfigureAwait(false);
            if (question == null)
            {
                return BadRequest();
            }

            var result = mapper.Map<QuestionGetResponseViewModel>(question);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            var answerResponse = await apiService.AnswerQuestion(requestViewModel.QuestionSetName, requestViewModel.QuestionNumber, requestViewModel.Answer).ConfigureAwait(false);

            if (answerResponse.IsSuccess)
            {
                if (answerResponse.IsComplete)
                {
                    return RedirectTo("assessment/complete");
                }
                else
                {
                    return RedirectTo($"assessment/{requestViewModel.QuestionSetName}/{answerResponse.NextQuestionNumber}");
                }
            }
            else
            {
                ModelState.AddModelError("Answer", "Failed to record answer");
                return View(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> New(string questionSetName)
        {
            var result = await apiService.NewSession(questionSetName).ConfigureAwait(false);

            if (result)
            {
                return RedirectTo($"assessment/{questionSetName}/1");
            }

            return RedirectTo($"assessment/{questionSetName}/1");
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

            if (viewModel.ReturnOption == 1)
            {
                return RedirectTo("assessment/email");
            }
            else
            {
                return RedirectTo("assessment/reference");
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
                TempData.Remove("Telephone");
                TempData.Add("Telephone", request.Telephone);
                return RedirectTo("assessment/referencesent");
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            return View(responseViewModel);
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }

        private async Task<AssessmentReferenceGetResponse> GetAssessmentViewModel()
        {
            var getAssessmentResponse = await GetAssessment().ConfigureAwait(false);

            var result = new AssessmentReferenceGetResponse();
            result.ReferenceCode = getAssessmentResponse.ReferenceCode;
            result.AssessmentStarted = getAssessmentResponse.StartedDt.ToString("d MMMM yyyy");

            return result;
        }

        private async Task<GetQuestionResponse> GetQuestion(string questionSetName, int questionNumber)
        {
            var question = await apiService.GetQuestion(questionSetName, questionNumber).ConfigureAwait(false);
            return question;
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

            return RedirectTo($"assessment/{assessment.QuestionSetName}/{assessment.NextQuestionNumber}");
        }
    }
}