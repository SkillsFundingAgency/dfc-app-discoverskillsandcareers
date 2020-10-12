using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class BodyTopTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly ILogService logService;

        public BodyTopTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            logService = A.Fake<ILogService>();

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService);
        }

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
