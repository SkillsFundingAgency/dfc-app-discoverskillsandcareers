using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.GraphQl;
using DFC.App.DiscoverSkillsCareers.MappingProfiles;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IDocumentStore documentStore;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly IGraphQlService graphQlService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

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
            documentStore = A.Fake<IDocumentStore>();
            var fakeMemoryCache = A.Fake<IMemoryCache>();
            staticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            graphQlService = A.Fake<IGraphQlService>();
            cmsApiClientOptions = new CmsApiClientOptions
            {
                ContentIds = Guid.NewGuid().ToString(),
            };

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, documentStore, fakeMemoryCache, staticContentDocumentService, graphQlService, cmsApiClientOptions);
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

            A.CallTo(() => documentStore.GetAllContentAsync<DysacJobProfileOverviewContentModel>(A<string>.Ignored, A<string>.Ignored)).Returns(new List<DysacJobProfileOverviewContentModel>() { new DysacJobProfileOverviewContentModel { Html = "<h1>Chemist</h1>", Title = "Chemist" } });
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);
            A.CallTo(() => resultsService.GetResultsByCategory(A<string>.Ignored)).Returns(new GetResultsResponse { JobCategories = new List<JobCategoryResult> { new JobCategoryResult { JobFamilyName = "testcategory", JobFamilyUrl = "testcategory", JobProfiles = new List<JobProfileResult>() { new JobProfileResult { Title = "Chemist" } } } } });

            var actionResponse = await controller.Roles(testCategory).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
