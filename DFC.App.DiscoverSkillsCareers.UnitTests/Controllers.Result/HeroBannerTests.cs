using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class HeroBannerTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly string testCategory;

        public HeroBannerTests()
        {
            mapper = A.Fake<IMapper>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            testCategory = "testCategory";
            Logger = A.Fake<ILogger<ResultsController>>();

            controller = new ResultsController(Logger, mapper, FakeSessionStateService, resultsService, assessmentService);
        }

        protected ILogger<ResultsController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            // Setup
            A.CallTo(() => controller.HasSessionId()).Returns(false);

            // Act
            var actionResponse = await controller.HeroBanner(testCategory).ConfigureAwait(false);

            // Asserts
            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("ACategory", false)]
        public async Task ShowsHeroBanner(string category, bool expectedCategory)
        {
            // Setup
            A.CallTo(() => controller.HasSessionId()).Returns(true);
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
