using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class HeroBannerTests
    {
        private readonly CompositeController controller;
        private readonly ISessionService sessionService;

        public HeroBannerTests()
        {
            sessionService = A.Fake<ISessionService>();
            controller = new CompositeController(sessionService, A.Fake<ILogger>());
        }

        [Fact]
        public void HeroBannerReturnsView()
        {
            var actionResponse = controller.HeroBanner();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void HeroBannerEmptyReturnsContentResult()
        {
            var actionResponse = controller.HeroBannerEmpty();
            Assert.IsType<ContentResult>(actionResponse);
        }
    }
}
