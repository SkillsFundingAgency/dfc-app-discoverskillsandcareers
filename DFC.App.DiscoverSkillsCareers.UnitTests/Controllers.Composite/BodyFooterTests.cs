using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class BodyFooterTests
    {
        private readonly CompositeController controller;
        private readonly ISessionService sessionService;

        public BodyFooterTests()
        {
            sessionService = A.Fake<ISessionService>();
            controller = new CompositeController(sessionService);
        }

        [Fact]
        public void BodyFooterReturnsView()
        {
            var actionResponse = controller.BodyFooter();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
