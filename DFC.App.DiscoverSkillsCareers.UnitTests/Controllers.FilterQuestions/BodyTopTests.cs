using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class BodyTopTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly IAssessmentService assessmentService;

        public BodyTopTests()
        {
            mapper = A.Fake<IMapper>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            assessmentService = A.Fake<IAssessmentService>();
            Logger = A.Fake<ILogger<FilterQuestionsController>>();

            controller = new FilterQuestionsController(Logger, mapper, FakeSessionStateService, assessmentService);
        }

        protected ILogger<FilterQuestionsController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public void BodyTopQuestionsReturnView()
        {
            // Setup
            var filterBodyTopViewModel = new FilterBodyTopViewModel() { QuestionNumber = 1 };

            // Act
            var actionResponse = controller.BodyTopQuestions(filterBodyTopViewModel);

            // Asserts
            var viewResult = Assert.IsType<ViewResult>(actionResponse);

            FilterBodyTopViewModel model = Assert.IsType<FilterBodyTopViewModel>(viewResult.Model);
            model.QuestionNumber.Should().Be(0);
        }

        [Fact]
        public void BodyTopQuestionsNullModelReturnsBadRequest()
        {
            // Setup
            FilterBodyTopViewModel filterBodyTopViewModel = null;

            // Act
            var actionResponse = controller.BodyTopQuestions(filterBodyTopViewModel);

            // Asserts
            Assert.IsType<BadRequestResult>(actionResponse);
        }

        [Fact]
        public void BodyTopCompleteReturnsBodyTopEmpty()
        {
            // Act
            var actionResponse = controller.BodyTopComplete();

            // Asserts
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
