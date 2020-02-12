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
                var result = CreateResponseViewModel();
                return View(result);
            }

            var question = await apiService.GetQuestion(requestViewModel.QuestionSetName, requestViewModel.QuestionNumber).ConfigureAwait(false);

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
                return Redirect("results");
            }

            if (requestViewModel.QuestionNumber > getAssessmentResponse.QuestionNumber)
            {
                return Redirect($"assessment/{requestViewModel.QuestionSetName}/{getAssessmentResponse.QuestionNumber}");
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

            var question = await apiService.GetQuestion(requestViewModel.QuestionSetName, requestViewModel.QuestionNumber).ConfigureAwait(false);
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
                    return Redirect("assessment/complete");
                }
                else
                {
                    return Redirect($"assessment/{requestViewModel.QuestionSetName}/{answerResponse.NextQuestionNumber}");
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
                return Redirect($"assessment/{questionSetName}/1");
            }

            return Redirect($"assessment/{questionSetName}/1");
        }

        public IActionResult Complete()
        {
            return View();
        }

        public async Task<IActionResult> Continue()
        {
            var assessment = await GetAssessment().ConfigureAwait(false);
            return NavigateTo(assessment);
        }

        public IActionResult Return()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Return(AssessmentReturnRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return Redirect($"assessment/short/02");
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
                return Redirect("assessment/email");
            }
            else
            {
                return Redirect("assessment/reference");
            }
        }

        public IActionResult Email()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Email(AssessmentEmailPostRequest request)
        {
            if (ModelState.IsValid)
            {
                await apiService.SendEmail($"https://{Request.Host.Value}", request.Email, "1").ConfigureAwait(false);

                return Redirect("assessment/emailsent");
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
            if (ModelState.IsValid)
            {
                TempData.Remove("Telephone");
                TempData.Add("Telephone", request.Telephone);
                return Redirect("assessment/referencesent");
            }

            var responseViewModel = await GetAssessmentViewModel().ConfigureAwait(false);
            return View(responseViewModel);
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }

        private QuestionGetResponseViewModel CreateResponseViewModel()
        {
            var result = new QuestionGetResponseViewModel();
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

        private async Task<GetQuestionResponse> GetQuestion(string questionSetName, int navigateToQuestionNumber)
        {
            var question = await apiService.GetQuestion(questionSetName, navigateToQuestionNumber).ConfigureAwait(false);
            return question;
        }

        private async Task<GetAssessmentResponse> GetAssessment()
        {
            var getAssessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);
            return getAssessmentResponse;
        }

        private async Task<IActionResult> NavigateTo(GetQuestionResponse question, int navigateToQuestionNumber)
        {
            if (question == null)
            {
                return BadRequest();
            }

            var getAssessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);

            if (getAssessmentResponse == null)
            {
                return BadRequest();
            }

            if (getAssessmentResponse.IsComplete)
            {
                return Redirect("results");
            }

            if (navigateToQuestionNumber > getAssessmentResponse.MaxQuestionsCount)
            {
                return BadRequest();
            }

            if (navigateToQuestionNumber > getAssessmentResponse.QuestionNumber)
            {
                return Redirect($"assessment/{question.QuestionSetName}/{getAssessmentResponse.QuestionNumber}");
            }

            return Redirect($"assessment/{question.QuestionSetName}/{navigateToQuestionNumber}");
        }

        private IActionResult NavigateTo(GetAssessmentResponse assessment)
        {
            if (assessment == null)
            {
                return BadRequest();
            }

            if (assessment.IsComplete)
            {
                return Redirect("results");
            }

            return Redirect($"assessment/{assessment.QuestionId}/{assessment.NextQuestionNumber}");
        }
    }
}