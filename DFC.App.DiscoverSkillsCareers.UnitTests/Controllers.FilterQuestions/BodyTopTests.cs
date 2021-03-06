﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class BodyTopTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly ILogService logService;

        public BodyTopTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            logService = A.Fake<ILogService>();

            controller = new FilterQuestionsController(logService, mapper, sessionService, assessmentService);
        }

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
