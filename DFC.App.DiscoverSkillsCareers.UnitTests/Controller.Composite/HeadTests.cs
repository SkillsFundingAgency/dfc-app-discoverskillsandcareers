using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Composite
{
    public class HeadTests
    {
        private readonly CompositeController controller;
        private readonly ISessionService sessionService;

        public HeadTests()
        {
            sessionService = A.Fake<ISessionService>();

            controller = new CompositeController(sessionService);
        }

        [Fact]
        public void IndexReturnsCorrectPageTitle()
        {
            var actionResult = controller.Index();
            AssertPageTitle(actionResult, PageTitle.Home);
        }

        [Fact]
        public void QuestionReturnsCorrectTitle()
        {
            var actionResult = controller.Question(1);
            AssertPageTitle(actionResult, "Q1");
        }

        [Fact]
        public void AssessmentSaveReturnsCorrectTitle()
        {
            var actionResult = controller.AssessmentSave();
            AssertPageTitle(actionResult, PageTitle.AssessmentSave);
        }

        [Fact]
        public void AssessmentReferenceReturnsCorrectTitle()
        {
            var actionResult = controller.AssessmentReference();
            AssertPageTitle(actionResult, PageTitle.AssessmentReference);
        }

        [Fact]
        public void AssessmentEmailReturnsCorrectTitle()
        {
            var actionResult = controller.AssessmentEmail();
            AssertPageTitle(actionResult, PageTitle.AssessmentEmail);
        }

        [Fact]
        public void AssessmentEmailSentReturnsCorrectTitle()
        {
            var actionResult = controller.AssessmentEmailSent();
            AssertPageTitle(actionResult, PageTitle.AssessmentEmailSent);
        }

        [Fact]
        public void ResultsReturnsCorrectTitle()
        {
            var actionResult = controller.Results();
            AssertPageTitle(actionResult, PageTitle.Results);
        }

        [Fact]
        public void AssessmentCompleteReturnsCorrectTitle()
        {
            var actionResult = controller.AssessmentComplete();
            AssertPageTitle(actionResult, PageTitle.AssessmentComplete);
        }

        private void AssertPageTitle(IActionResult actionResult, string expectedTitle)
        {
            Assert.IsType<ViewResult>(actionResult);

            var viewResult = actionResult as ViewResult;
            var vm = viewResult.Model as HeadResponseViewModel;
            Assert.Equal($"{expectedTitle} | {PageTitle.Dysac} | {PageTitle.Ncs}", vm.Title);
        }
    }
}
