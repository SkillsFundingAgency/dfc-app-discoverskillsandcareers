﻿using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class PageTitleTests
    {
        private readonly CompositeController controller;
        private readonly ISessionService sessionService;

        public PageTitleTests()
        {
            sessionService = A.Fake<ISessionService>();
            controller = new CompositeController(sessionService);
        }

        [Fact]
        public void IndexReturnsDefaultTitle()
        {
            var actionResponse = controller.Index();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void QuestionReturnsPageTitle()
        {
            var questionNumber = 1;
            var actionResponse = controller.Question(questionNumber);
            AssertPageTitle(actionResponse, $"Q{questionNumber}");
        }

        [Fact]
        public void AssessmentSaveReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentSave();
            AssertPageTitle(actionResponse, PageTitle.AssessmentSave);
        }

        [Fact]
        public void AssessmentReferenceReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentReference();
            AssertPageTitle(actionResponse, PageTitle.AssessmentReference);
        }

        [Fact]
        public void AssessmentEmailReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentEmail();
            AssertPageTitle(actionResponse, PageTitle.AssessmentEmail);
        }

        [Fact]
        public void AssessmentEmailSentReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentEmailSent();
            AssertPageTitle(actionResponse, PageTitle.AssessmentEmailSent);
        }

        [Fact]
        public void AssessmentReturnReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentReturn();
            AssertPageTitle(actionResponse, PageTitle.AssessmentReturn);
        }

        [Fact]
        public void ResultsReturnsPageTitle()
        {
            var actionResponse = controller.Results();
            AssertPageTitle(actionResponse, PageTitle.Results);
        }

        [Fact]
        public void AssessmentCompleteReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentComplete();
            AssertPageTitle(actionResponse, PageTitle.AssessmentComplete);
        }

        private void AssertPageTitle(object actionResponse, string expectedPageTitle)
        {
            Assert.IsType<ViewResult>(actionResponse);
            var viewResult = actionResponse as ViewResult;
            var model = viewResult.Model as HeadResponseViewModel;
            Assert.Equal($"{expectedPageTitle} | {PageTitle.Dysac} | {PageTitle.Ncs}", model.Title);
        }
    }
}
