using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class BodyFooterTests
    {
        private readonly CompositeController controller;

        public BodyFooterTests()
        {
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            Logger = A.Fake<ILogger<CompositeController>>();

            controller = new CompositeController(Logger, FakeSessionStateService);
        }

        protected ILogger<CompositeController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public void BodyFooterReturnsView()
        {
            var actionResponse = controller.BodyFooter();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
