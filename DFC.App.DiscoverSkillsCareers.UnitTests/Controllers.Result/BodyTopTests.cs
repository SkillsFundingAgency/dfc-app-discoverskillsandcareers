using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
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

        public BodyTopTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();

            controller = new ResultsController(mapper, sessionService, resultsService, assessmentService);
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
