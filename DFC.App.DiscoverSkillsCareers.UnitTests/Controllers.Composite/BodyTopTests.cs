using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            controller = new CompositeController(sessionService, A.Fake<ILogService>());
        }

        [Fact]
        public void BodyTopReturnsView()
        {
            var actionResponse = controller.BodyTop();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BreadcrumbsHomeReturnsView()
        {
            var actionResponse = controller.BreadcrumbsHome();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BreadcrumbsSaveProgressReturnsView()
        {
            var actionResponse = controller.BreadcrumbsSaveProgress();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BreadcrumbsReferenceCodeReturnsView()
        {
            var actionResponse = controller.BreadcrumbsReferenceCode();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BreadcrumbsQuestionsCompleteReturnsView()
        {
            var actionResponse = controller.BreadcrumbsQuestionsComplete();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BreadcrumbsQuestionsReturnsView()
        {
            var assessmentType = "short";
            var questionNumber = 2;
            var actionResponse = controller.BreadcrumbsQuestions(assessmentType, questionNumber);
            var viewResult = actionResponse as ViewResult;
            var model = viewResult.Model as BodyTopQuestionsViewModel;

            Assert.IsType<ViewResult>(actionResponse);
            Assert.Equal(questionNumber - 1, model.PreviousQuestionNumber);
            Assert.Equal(assessmentType, model.AssessmentType);
        }
    }
}
