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

            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            var assessment = await apiService.GetAssessment().ConfigureAwait(false);
            if (!assessment.IsComplete)
            {
                return RedirectTo("assessment/return");
            }

            var response = await GetQuestion(viewModel.JobCategoryName, viewModel.QuestionNumber).ConfigureAwait(false);
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Index(FilterQuestionPostRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            if (!HasSessionId())
            {
                return RedirectToRoot();
            }

            if (!ModelState.IsValid)
            {
                var response = await GetQuestion(viewModel.JobCategoryName, viewModel.QuestionNumberCounter).ConfigureAwait(false);
                return View(response);
            }

            var answerResponse = await apiService.AnswerQuestion(viewModel.AssessmentType, viewModel.QuestionNumberReal, viewModel.Answer).ConfigureAwait(false);

            if (answerResponse == null)
            {
                return BadRequest();
            }

            if (!answerResponse.IsSuccess)
            {
                return BadRequest();
            }

            if (answerResponse.IsComplete)
            {
                return RedirectTo("results");
            }

            return RedirectTo($"{viewModel.AssessmentType}/filterquestions/{viewModel.JobCategoryName}/{answerResponse.NextQuestionNumber}");
        }

        public IActionResult Complete(FilterQuestionsCompleteResponseViewModel viewModel)
        {
            return View(viewModel);
        }

        private async Task<FilterQuestionIndexResponseViewModel> GetQuestion(string assessment, int questionNumber)
        {
            var filtereredQuestion = await apiService.GetQuestion(assessment, questionNumber).ConfigureAwait(false);
            var response = new FilterQuestionIndexResponseViewModel();
            response.Question = mapper.Map<QuestionGetResponseViewModel>(filtereredQuestion);
            response.JobCategoryName = assessment;
            return response;
        }
    }
}
