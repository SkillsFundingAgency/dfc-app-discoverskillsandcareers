using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class PageTitleTests
    {
        private readonly CompositeController controller;

        public PageTitleTests()
        {
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            Logger = A.Fake<ILogger<CompositeController>>();

            controller = new CompositeController(Logger, FakeSessionStateService);
        }

        protected ILogger<CompositeController> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

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
        public void AssessmentReferenceSentReturnsPageTitle()
        {
            var actionResponse = controller.AssessmentReferenceSent();
            AssertPageTitle(actionResponse, PageTitle.AssessmentReferenceSent);
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

        [Fact]
        public void LoadSessionReturnsPageTitle()
        {
            var actionResponse = controller.LoadSession();
            AssertPageTitle(actionResponse, PageTitle.LoadSession);
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
