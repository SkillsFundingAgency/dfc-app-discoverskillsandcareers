using DFC.App.DiscoverSkillsCareers.Services;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class FilterQuestionsController : BaseController
    {
        private readonly QuestionSetDataProvider questionSetDataProvider;

        public FilterQuestionsController()
        {
            questionSetDataProvider = new QuestionSetDataProvider();
        }

        [HttpGet]
        public IActionResult Index(FilterQuestionGetRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            var result = CreateResponseViewModel(viewModel.JobCategoryName, viewModel.QuestionId);
            return View(result);
        }

        [HttpPost]
        public IActionResult Index(FilterQuestionPostRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            var result = CreateResponseViewModel(viewModel.JobCategoryName, viewModel.QuestionId);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            if (string.IsNullOrWhiteSpace(result.NextQuestionId))
            {
                return Redirect($"filterquestions/{result.JobCategoryName}/complete");
            }

            return Redirect($"filterquestions/{result.JobCategoryName}/{result.NextQuestionId}");
        }

        public IActionResult Complete(FilterQuestionsCompleteResponseViewModel viewModel)
        {
            return View(viewModel);
        }

        private FilterQuestionGetResponseViewModel CreateResponseViewModel(string questionSetName, string questionId)
        {
            var result = new FilterQuestionGetResponseViewModel() { JobCategoryName = questionSetName };

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
                    var totalquestions = questionSet.Questions.Count;
                    result.PercentageComplete = (decimal)totalCompleted / totalquestions;
                }
            }

            return result;
        }
    }
}
