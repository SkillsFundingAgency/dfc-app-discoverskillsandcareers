using AutoMapper;
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

        public IActionResult Continue()
        {
            return Redirect("assessment/short/02");
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

            return RedirectToAction("index", "question", new { QuestionSetName = "short", QuestionId = "02" });
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
                return RedirectToAction("Email");
            }
            else
            {
                return RedirectToAction("Reference");
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

                return RedirectToAction("EmailSent");
            }

            return View(request);
        }

        public IActionResult EmailSent()
        {
            return View();
        }

        public async Task<IActionResult> Reference()
        {
            var getAssessmentResponse = await apiService.GetAssessment().ConfigureAwait(false);

            var responseViewModel = new AssessmentReferenceGetResponse();
            responseViewModel.ReferenceCode = getAssessmentResponse.ReferenceCode;
            responseViewModel.AssessmentStarted = getAssessmentResponse.StartedDt.ToString("d MMMM yyyy");

            return View(responseViewModel);
        }

        [HttpPost]
        public IActionResult Reference(AssessmentReferencePostRequest request)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ReferenceSent");
            }

            return View();
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
    }
}