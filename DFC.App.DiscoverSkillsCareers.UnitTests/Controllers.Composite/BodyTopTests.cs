﻿using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class BodyTopTests
    {
        private readonly CompositeController controller;
        private readonly ISessionService sessionService;

        public BodyTopTests()
        {
            sessionService = A.Fake<ISessionService>();
            controller = new CompositeController(sessionService);
        }

        [Fact]
        public void ReturnsView()
        {
            var actionResponse = controller.BodyTop();
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
