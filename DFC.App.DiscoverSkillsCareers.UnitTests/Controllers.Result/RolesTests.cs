using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.MappingProfiles;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
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
        private readonly IRazorTemplateEngine razorTemplateEngine;
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;

        public RolesTests()
        {
            var profile = new ResultProfileOverviewsProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(configuration);

            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            testCategory = "testcategory";
            logService = A.Fake<ILogService>();
            cmsApiClientOptions = new CmsApiClientOptions
            {
                ContentIds = Guid.NewGuid().ToString(),
            };
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            razorTemplateEngine = A.Fake<IRazorTemplateEngine>();

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, cmsApiClientOptions, sharedContentRedisInterface, razorTemplateEngine);
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
            A.CallTo(() => resultsService.GetResultsByCategory(A<string>.Ignored)).Returns(new GetResultsResponse { JobCategories = new List<JobCategoryResult> { new JobCategoryResult { JobFamilyName = "testcategory", JobFamilyUrl = "testcategory", JobProfiles = new List<JobProfileResult>() { new JobProfileResult { Title = "Chemist" } } } } });

            var actionResponse = await controller.Roles(testCategory).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
