﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Controllers
{
    public class FilterQuestionsController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAssessmentService apiService;
        private readonly ILogService logService;

        public FilterQuestionsController(ILogService logService, IMapper mapper, ISessionService sessionService, IAssessmentService apiService)
            : base(sessionService)
        {
            this.logService = logService;
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

            if (viewModel.QuestionNumber == 0)
            {
                var filterAssessment = await apiService.FilterAssessment(viewModel.JobCategoryName).ConfigureAwait(false);

                return RedirectTo(
                    $"{viewModel.AssessmentType}/filterquestions/{viewModel.JobCategoryName}/{filterAssessment.QuestionNumber}");
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

            var answerResponse = await apiService.AnswerFilterQuestion(
                viewModel.JobCategoryName,
                viewModel.QuestionNumberReal,
                viewModel.QuestionNumberCounter,
                viewModel.Answer).ConfigureAwait(false);

            if (!answerResponse.IsSuccess)
            {
                ModelState.AddModelError("Answer", "Unable to register answer");
                var response = await GetQuestion(viewModel.JobCategoryName, viewModel.QuestionNumberCounter).ConfigureAwait(false);

                return View(response);
            }

            if (answerResponse.IsComplete)
            {
                return RedirectTo($"{viewModel.AssessmentType}/filterquestions/{viewModel.JobCategoryName}/complete");
            }

            return RedirectTo($"{viewModel.AssessmentType}/filterquestions/{viewModel.JobCategoryName}/{viewModel.QuestionNumberCounter + 1}");
        }

        public async Task<IActionResult> Complete(FilterQuestionsCompleteResponseViewModel viewModel)
        {
            var hasSessionId = await HasSessionId().ConfigureAwait(false);
            return !hasSessionId ? RedirectToRoot() : View(viewModel);
        }

        [HttpGet]
        [Route("bodytop/{assessmentType}/filterquestions/{jobCategoryName}/{QuestionNumber}")]
        public IActionResult BodyTopQuestions(FilterBodyTopViewModel resultsBodyTopViewModel)
        {
            if (resultsBodyTopViewModel == null)
            {
                return BadRequest();
            }

            resultsBodyTopViewModel.QuestionNumber -= 1;
            logService.LogInformation($"{nameof(this.BodyTopQuestions)} generated the model and ready to pass to the view");

            return View(resultsBodyTopViewModel);
        }

        [HttpGet]
        [Route("bodytop/{assessmentType}/filterquestions/{jobCategoryName}/complete")]
        public IActionResult BodyTopComplete()
        {
            return View("BodyTopEmpty");
        }

        private async Task<FilterQuestionIndexResponseViewModel> GetQuestion(string assessment, int questionNumber)
        {
            var filteredQuestion = await apiService.GetFilteredAssessmentQuestion(assessment, questionNumber).ConfigureAwait(false);
            var response = new FilterQuestionIndexResponseViewModel
            {
                Question = mapper.Map<QuestionGetResponseViewModel>(filteredQuestion),
                JobCategoryName = assessment,
            };

            logService.LogInformation($"{nameof(this.GetQuestion)} generated the model and ready to pass to the view");
            return response;
        }
    }
}
