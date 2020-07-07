using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class IndexTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;

        public IndexTests()
        {
            mapper = A.Fake<IMapper>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            Logger = A.Fake<ILogger<ResultsController>>();

            controller = new ResultsController(Logger, mapper, FakeSessionStateService, resultsService, assessmentService);
        }

        protected ILogger<ResultsController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            A.CallTo(() => controller.HasSessionId()).Returns(false);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsNotCompleteRedirectsToAssessment()
        {
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 1 };
            A.CallTo(() => controller.HasSessionId()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task WhenHasPreviousCompleteCategoryRedirectsToRoles()
        {
            var category = "testcategory";
            var assessmentResponse = new GetAssessmentResponse() { IsFilterAssessment = true,  MaxQuestionsCount = 2, RecordedAnswersCount = 2,  };
            A.CallTo(() => controller.HasSessionId()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var resultsResponse = new GetResultsResponse() { JobCategories = GetJobCategories(category) };

            A.CallTo(() => resultsService.GetResults()).Returns(resultsResponse);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;

            Assert.Equal($"~/{RouteName.Prefix}/results/roles/{category}", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsCompleteShowsResults()
        {
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 2 };

            A.CallTo(() => controller.HasSessionId()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        private IEnumerable<JobCategoryResult> GetJobCategories(string category)
        {
            yield return new JobCategoryResult() { JobFamilyName = category,   FilterAssessment = new FilterAssessmentResult() { CreatedDt = DateTime.Now } };
        }
    }
}
