using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Home
{
    public class LoadSessionGetTests
    {
        private readonly TestController controller;
        private readonly IAssessmentService assessmentService;

        public LoadSessionGetTests()
        {
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            Logger = A.Fake<ILogger<TestController>>();

            controller = new TestController(Logger, FakeSessionStateService, assessmentService);
        }

        protected ILogger<TestController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public void ReturnsViewResultWhenCalled()
        {
            var actionResponse = controller.LoadSession();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
