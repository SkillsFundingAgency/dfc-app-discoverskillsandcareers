﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class HeroBannerTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly string testCategory;
        private readonly ILogService logService;
        private readonly IDocumentStore documentStore;

        public HeroBannerTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            testCategory = "testCategory";
            logService = A.Fake<ILogService>();
            documentStore = A.Fake<IDocumentStore>();
            var fakeMemoryCache = A.Fake<IMemoryCache>();

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, documentStore, fakeMemoryCache);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            // Setup
            A.CallTo(() => sessionService.HasValidSession()).Returns(false);

            // Act
            var actionResponse = await controller.HeroBanner(testCategory).ConfigureAwait(false);

            // Asserts
            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("ACategory", true)]
        public async Task ShowsHeroBanner(string category, bool expectedCategory)
        {
            // Setup
            A.CallTo(() => sessionService.HasValidSession()).Returns(true);
            A.CallTo(() => mapper.Map<ResultsHeroBannerViewModel>(A<GetResultsResponse>.Ignored)).Returns(new ResultsHeroBannerViewModel());

            // Act
            var actionResponse = await controller.HeroBanner(category).ConfigureAwait(false);

            // Asserts
            var viewResult = Assert.IsType<ViewResult>(actionResponse);

            ResultsHeroBannerViewModel model = Assert.IsType<ResultsHeroBannerViewModel>(viewResult.Model);
            model.IsCategoryBanner.Should().Be(expectedCategory);
        }
    }
}
