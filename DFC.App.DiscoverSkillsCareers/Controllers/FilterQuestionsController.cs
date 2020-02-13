using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class FilterQuestionsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IApiService apiService;

        public FilterQuestionsController(IMapper mapper, ISessionService sessionService, IApiService apiService)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(FilterQuestionIndexRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            var filterAssessmentResponse = await apiService.FilterAssessment(viewModel.JobCategoryName).ConfigureAwait(false);
            var filtereredQuestion = await apiService.GetQuestion(viewModel.JobCategoryName, viewModel.QuestionNumber);

            var response = new FilterQuestionIndexResponseViewModel();
            response.Question = mapper.Map<QuestionGetResponseViewModel>(filtereredQuestion);
            response.JobCategoryName = viewModel.JobCategoryName;

            return View(response);
        }

        [HttpPost]
        public IActionResult Index(FilterQuestionPostRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            var result = CreateResponseViewModel(viewModel.JobCategoryName, string.Empty);

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
