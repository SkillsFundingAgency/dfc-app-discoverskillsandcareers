﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class FilterQuestionsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAssessmentService apiService;

        public FilterQuestionsController(IMapper mapper, ISessionService sessionService, IAssessmentService apiService)
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

            var hasSessionId = await HasSessionId().ConfigureAwait(false);

            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            var assessment = await apiService.GetAssessment().ConfigureAwait(false);
            if (!assessment.IsComplete && !assessment.IsFilterAssessment)
            {
                return RedirectTo("assessment/return");
            }

            var filterAssessment = await apiService.FilterAssessment(viewModel.JobCategoryName).ConfigureAwait(false);
            var response = await GetQuestion(viewModel.JobCategoryName, filterAssessment.QuestionNumber).ConfigureAwait(false);
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Index(FilterQuestionPostRequestViewModel viewModel)
        {
            if (viewModel == null)
            {
                return BadRequest();
            }

            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            if (!ModelState.IsValid)
            {
                var response = await GetQuestion(viewModel.JobCategoryName, viewModel.QuestionNumberCounter).ConfigureAwait(false);
                return View(response);
            }

            var answerResponse = await apiService.AnswerQuestion(viewModel.JobCategoryName, viewModel.QuestionNumberReal, viewModel.QuestionNumberCounter, viewModel.Answer).ConfigureAwait(false);

            if (answerResponse == null)
            {
                return BadRequest();
            }

            if (!answerResponse.IsSuccess)
            {
                ModelState.AddModelError("Answer", "Unable to register answer");
                var response = await GetQuestion(viewModel.JobCategoryName, viewModel.QuestionNumberCounter).ConfigureAwait(false);
                return View(response);
            }

            if (answerResponse.IsComplete)
            {
                return View("complete", new FilterQuestionsCompleteResponseViewModel() { JobCategoryName = viewModel.JobCategoryName });
            }

            return RedirectTo($"{viewModel.AssessmentType}/filterquestions/{viewModel.JobCategoryName}/{answerResponse.NextQuestionNumber}");
        }

        public async Task<IActionResult> Complete(FilterQuestionsCompleteResponseViewModel viewModel)
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            if (!hasSessionId)
            {
                return RedirectToRoot();
            }

            return View(viewModel);
        }

        private async Task<FilterQuestionIndexResponseViewModel> GetQuestion(string assessment, int questionNumber)
        {
            var filtereredQuestion = await apiService.GetQuestion(assessment, questionNumber).ConfigureAwait(false);
            var response = new FilterQuestionIndexResponseViewModel
            {
                Question = mapper.Map<QuestionGetResponseViewModel>(filtereredQuestion),
                JobCategoryName = assessment,
            };
            return response;
        }
    }
}
