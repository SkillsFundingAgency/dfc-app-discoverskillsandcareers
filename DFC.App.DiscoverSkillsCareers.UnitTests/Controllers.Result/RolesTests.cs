using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.MappingProfiles;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Mapping;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class RolesTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly string testCategory;
        private readonly ILogService logService;

        public RolesTests()
        {
            var profile = new ResultProfileOverviewsProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(configuration);

            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            testCategory = "testCategory";
            logService = A.Fake<ILogService>();

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            A.CallTo(() => sessionService.HasValidSession()).Returns(false);

            var actionResponse = await controller.Roles(testCategory).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsNotCompleteRedirectsToAssessment()
        {
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 1 };
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Roles(testCategory).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsCompleteShowsResults()
        {
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 2 };

            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => resultsService.GetResultsByCategory(A<string>.Ignored)).Returns(new GetResultsResponse { JobCategories = new List<JobCategoryResult> { new JobCategoryResult { JobFamilyName = "testCategory", JobFamilyUrl = "testCategory", JobProfiles = new List<JobProfileResult>() } } });

            var actionResponse = await controller.Roles(testCategory).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
