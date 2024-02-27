using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.GraphQl;
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
    public class IndexTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly ILogService logService;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine fakeRazorTemplateEngine;

        public IndexTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            logService = A.Fake<ILogService>();
            staticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            cmsApiClientOptions = new CmsApiClientOptions
            {
                ContentIds = Guid.NewGuid().ToString(),
            };
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, staticContentDocumentService, cmsApiClientOptions, sharedContentRedisInterface, fakeRazorTemplateEngine);
        }

        [Fact]
        public void NullContentIdThrowsException()
        {
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, staticContentDocumentService, new CmsApiClientOptions(), sharedContentRedisInterface, fakeRazorTemplateEngine));

            // Assert
            Assert.Equal("ContentIds cannot be null (Parameter 'cmsApiClientOptions')", ex.Message);
        }

        [Fact]
        public void NullCmsApiClientOptionsThrowsException()
        {
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, staticContentDocumentService, null, sharedContentRedisInterface, fakeRazorTemplateEngine));

            // Assert
            Assert.Equal("ContentIds cannot be null (Parameter 'cmsApiClientOptions')", ex.Message);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            A.CallTo(() => sessionService.HasValidSession()).Returns(false);

            var actionResponse = await controller.Index(null).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenHasPreviousCompleteCategoryRedirectsToRoles()
        {
            var category = "testcategory";
            var assessmentResponse = new GetAssessmentResponse() { IsFilterAssessment = true, MaxQuestionsCount = 2, RecordedAnswersCount = 2, };
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var resultsResponse = new GetResultsResponse() { JobCategories = GetJobCategories(category), LastAssessmentCategory = category };

            A.CallTo(() => resultsService.GetResults(true)).Returns(resultsResponse);

            var actionResponse = await controller.Index(null).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;

            Assert.Equal($"~/{RouteName.Prefix}/results/roles/{category}", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsCompleteShowsResults()
        {
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 2 };

            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index(null).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }

        private IEnumerable<JobCategoryResult> GetJobCategories(string category)
        {
            yield return new JobCategoryResult() { JobFamilyName = category };
        }
    }
}
