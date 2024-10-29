using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
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
            controller = new CompositeController(sessionService, A.Fake<ILogService>());
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

        [Fact]
        public void HeroBannerReturnsViewForStart()
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            routeData.Values["data"] = "start";
            var actionContext = new ActionContext(httpContext, routeData, new ControllerActionDescriptor());
            controller.ControllerContext = new ControllerContext(actionContext);
            var actionResponse = controller.HeroBannerEmpty();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
