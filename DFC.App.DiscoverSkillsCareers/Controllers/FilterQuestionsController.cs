using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class FilterQuestionsController : BaseController
    {
        public FilterQuestionsController(ISessionService sessionService)
            : base(sessionService)
        {
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

            var result = CreateResponseViewModel(viewModel.JobCategoryName, "");

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

            return result;
        }
    }
}
