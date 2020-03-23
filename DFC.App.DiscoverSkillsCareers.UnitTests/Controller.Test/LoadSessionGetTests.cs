using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class LoadSessionGetTests
    {
        private readonly TestController controller;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;

        public LoadSessionGetTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();

            controller = new TestController(sessionService, assessmentService);
        }

        [Fact]
        public void ReturnsViewResultWhenCalled()
        {
            var actionResponse = controller.LoadSession();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
