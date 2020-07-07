using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class BodyTopTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;

        public BodyTopTests()
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
        public void BodyTopCompleteReturnsBodyTopEmpty()
        {
            // Act
            var actionResponse = controller.BodyTop();

            // Asserts
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
