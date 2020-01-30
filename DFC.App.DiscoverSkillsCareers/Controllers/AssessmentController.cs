using DFC.App.DiscoverSkillsCareers.Constants;
using DFC.App.DiscoverSkillsCareers.Services;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class AssessmentController : Controller
    {
        private QuestionSetDataProvider questionSetDataProvider;

        public AssessmentController()
        {
            questionSetDataProvider = new QuestionSetDataProvider();
        }

        [HttpGet]
        public IActionResult Index(QuestionGetRequestViewModel viewModel)
        {
            var result = CreateResponseViewModel(viewModel.QuestionSetName, viewModel.QuestionId);
            return View(result);
        }

        [HttpPost]
        public IActionResult Index(QuestionPostRequestViewModel answerViewModel)
        {
            var result = CreateResponseViewModel(answerViewModel.QuestionSetName, answerViewModel.QuestionId);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            if (string.IsNullOrWhiteSpace(result.NextQuestionId))
            {
                return Redirect("assessment/complete");
            }

            //Save answer
            return Redirect($"assessment/{result.QuestionSetName}/{result.NextQuestionId}");
        }

        public IActionResult New(string questionSetName)
        {
            return Redirect($"assessment/{questionSetName}/01");
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
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.ReturnOption == 1)
                return RedirectToAction("Email");
            else
                return RedirectToAction("Reference");
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

        public override RedirectResult Redirect(string url)
        {
            url = $"~/{RouteName.Prefix}/" + url;
            return base.Redirect(url);
        }

        private QuestionGetResponseViewModel CreateResponseViewModel(string questionSetName, string questionId)
        {
            var result = new QuestionGetResponseViewModel();

            var questionSet = questionSetDataProvider.GetQuestionSet(questionSetName);
            if (questionSet != null)
            {
                result.QuestionSetName = questionSet.Name;

                var question = questionSet.GetQuestion(questionId);

                if (question != null)
                {
                    result.QuestionId = question.Id;
                    result.QuestionText = question.Text;
                    result.IsComplete = questionSet.IsCompleted();

                    var prevQuestion = questionSet.GetPreviousQuestion(questionId);
                    var nextQuestion = questionSet.GetNextQuestion(questionId);

                    if (prevQuestion != null)
                    {
                        result.PreviousQuestionId = prevQuestion.Id;
                    }
                    if (nextQuestion != null)
                    {
                        result.NextQuestionId = nextQuestion.Id;
                    }

                    var totalCompleted = questionSet.GetCompleted();
                    var totalquestions = questionSet.Questions.Count();
                    result.PercentageComplete = (decimal)totalCompleted / totalquestions;
                }
            }

            return result;
        }
    }
}
