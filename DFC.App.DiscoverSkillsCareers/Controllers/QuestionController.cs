using DFC.App.DiscoverSkillsCareers.Services;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class QuestionController : Controller
    {
        private QuestionSetDataProvider questionSetDataProvider;

        public QuestionController()
        {
            questionSetDataProvider = new QuestionSetDataProvider();
        }

        [Route("{QuestionSetName}/{QuestionId}")]
        [HttpGet]
        public IActionResult Index(QuestionGetRequestViewModel viewModel)
        {
            var result = CreateResponseViewModel(viewModel.QuestionSetName, viewModel.QuestionId);
            return View(result);
        }

        [Route("{QuestionSetName}/{QuestionId}")]
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
                return RedirectToAction("finish", "assessment");
            }

            //Save answer
            return RedirectToAction("Index", new { result.QuestionSetName, QuestionId = result.NextQuestionId });
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
