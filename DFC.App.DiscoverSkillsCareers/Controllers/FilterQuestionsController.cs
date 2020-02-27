using AutoMapper;
using Dfc.Session;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class FilterQuestionsController : BaseController
    {
        private const string ErrorKey = "Answer";
        private const string ErrorMessage = "Unable to register answer";
        private readonly IMapper mapper;
        private readonly IResultsService<ShortAssessment> resultsService;

        public FilterQuestionsController(IMapper mapper, ISessionClient sessionService, IResultsService<ShortAssessment> resultsService)
            : base(sessionService)
        {
            this.mapper = mapper;
            this.resultsService = resultsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(FilterQuestionIndexRequestViewModel filterRequest)
        {
            if (filterRequest is null)
            {
                return BadRequest();
            }

            if (!await HasSessionIdAsync().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            if (!await resultsService.IsAssessmentComplete().ConfigureAwait(false))
            {
                return RedirectToAction(nameof(ShortAssessmentController.Return), nameof(ShortAssessmentController));
            }

            var filtereredQuestion = await resultsService.GetFilterQuestion(filterRequest.JobCategoryName, filterRequest.QuestionNumber).ConfigureAwait(false);
            var viewModel = new FilterQuestionIndexResponseViewModel
            {
                Question = mapper.Map<QuestionGetResponseViewModel>(filtereredQuestion),
                JobCategoryName = filterRequest.JobCategoryName,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(FilterQuestionPostRequestViewModel filterQuestionResponse)
        {
            if (filterQuestionResponse is null)
            {
                return BadRequest();
            }

            if (!await HasSessionIdAsync().ConfigureAwait(false))
            {
                return RedirectToRoot();
            }

            if (!ModelState.IsValid)
            {
                var filtereredQuestion = await resultsService.GetFilterQuestion(filterQuestionResponse.JobCategoryName, filterQuestionResponse.QuestionNumberCounter).ConfigureAwait(false);
                return View(filtereredQuestion);
            }

            var answerResponse = await resultsService.AnswerFilterQuestion(filterQuestionResponse.QuestionNumberReal, filterQuestionResponse.Answer).ConfigureAwait(false);
            if (answerResponse is null)
            {
                //Dont think bad request might be the correct answer?
                return BadRequest();
            }

            if (!answerResponse.IsSuccess)
            {
                ModelState.AddModelError(ErrorKey, ErrorMessage);
                return View(answerResponse.Question);
            }

            if (answerResponse.IsComplete)
            {
                return RedirectToAction(nameof(ResultsController.Index));
            }

            return RedirectTo($"{filterQuestionResponse.AssessmentType}/filterquestions/{filterQuestionResponse.JobCategoryName}/{answerResponse.NextQuestionNumber}");
        }

        public IActionResult Complete(FilterQuestionsCompleteResponseViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}