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
        public void BodyTopEmptyReturnsView()
        {
            var actionResponse = controller.BodyTopEmpty();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BodyTopFirstQuestionReturnsView()
        {
            var actionResponse = controller.BodyTopBackToStart();
            Assert.IsType<ViewResult>(actionResponse);
        }

        [Fact]
        public void BodyTopQuestionsReturnsView()
        {
            var assessmentType = "short";
            var questionNumber = 2;
            var actionResponse = controller.BodyTopQuestions(assessmentType, questionNumber);
            Assert.IsType<ViewResult>(actionResponse);

            var viewResult = actionResponse as ViewResult;
            var model = viewResult.Model as BodyTopQuestionsViewModel;
            Assert.Equal(questionNumber - 1, model.PreviousQuestionNumber);
            Assert.Equal(assessmentType, model.AssessmentType);
        }
    }
}
