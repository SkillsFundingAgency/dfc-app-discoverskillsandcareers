using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Dfc.Session;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class IndexTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISession session;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;


        public IndexTests()
        {
            mapper = A.Fake<IMapper>();
            session = A.Fake<ISession>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();

            controller = new ResultsController(mapper, session, resultsService, assessmentService);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            string sessionId = null;
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsNotCompleteRedirectsToAssessment()
        {
            var sessionId = "session1";
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 1 };
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/assessment/return", redirectResult.Url);
        }

        [Fact]
        public async Task WhenAssessmentIsCompleteShowsResults()
        {
            var sessionId = "session1";
            var assessmentResponse = new GetAssessmentResponse() { MaxQuestionsCount = 2, RecordedAnswersCount = 2 };
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
