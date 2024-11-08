using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class ReturnViewTests
    {
        private readonly CompositeController controller;
        private readonly ISessionService sessionService;

        public ReturnViewTests()
        {
            sessionService = A.Fake<ISessionService>();
            controller = new CompositeController(sessionService, A.Fake<ILogService>());
        }

        [Fact]
        public void ReferenceSentTest()
        {
            var actionResponse = controller.ReferenceSent();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void EmailSentTest()
        {
            var actionResponse = controller.EmailSent();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
