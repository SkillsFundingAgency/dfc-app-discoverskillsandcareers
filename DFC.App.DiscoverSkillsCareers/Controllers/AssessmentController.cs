using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class AssessmentController : BaseController
    {
        private readonly IApiService apiService;

        public AssessmentController(ISessionService sessionService, IApiService apiService)
            : base(sessionService)
        {
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

            var responseViewModel = CreateResponseViewModel(question);
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionPostRequestViewModel requestViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var answerResponse = await apiService.AnswerQuestion(requestViewModel.QuestionSetName, requestViewModel.QuestionNumber, requestViewModel.Answer);

            if (answerResponse.IsSuccess && answerResponse.IsComplete)
            {
                return Redirect("assessment/finish");
            }

            if (answerResponse.IsSuccess)
            {
                return Redirect($"assessment/{requestViewModel.QuestionSetName}/{answerResponse.NextQuestionNumber}");
            }

            return View();
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
        public IActionResult Email(AssessmentEmailPostRequest request)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("EmailSent");
            }

            return View(request);
        }

        public IActionResult EmailSent()
        {
            return View();
        }

        public IActionResult Reference()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reference(AssessmentReferencePostRequest request)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ReferenceSent");
            }

            return View(request);
        }

        public IActionResult ReferenceSent()
        {
            return View();
        }

        private async Task<GetQuestionResponse> GetQuestion(string assessment, int questionNumber)
        {
            var result = await apiService.GetQuestion(assessment, questionNumber);
            return result;
        }

        private QuestionGetResponseViewModel CreateResponseViewModel()
        {
            var result = new QuestionGetResponseViewModel();
            return result;
        }

        private QuestionGetResponseViewModel CreateResponseViewModel(string assessment, int questionNumber)
        {
            var result = new QuestionGetResponseViewModel();

            return result;
        }

        private QuestionGetResponseViewModel CreateResponseViewModel(GetQuestionResponse question)
        {
            var result = new QuestionGetResponseViewModel();

            result.IsComplete = question.IsComplete;
            result.NextQuestionNumber = question.NextQuestionNumber;
            result.PercentageComplete = question.PercentComplete;
            result.PreviousQuestionId = question.PreviousQuestionNumber;
            result.QuestionId = question.QuestionId;
            result.QuestionSetName = question.QuestionSetName;
            result.QuestionText = question.QuestionText;

            return result;
        }
    }
}