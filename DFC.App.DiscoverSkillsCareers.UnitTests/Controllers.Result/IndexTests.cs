using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
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
        private readonly ISessionService sessionService;
        private readonly IApiService apiService;

        public IndexTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            apiService = A.Fake<IApiService>();

            controller = new ResultsController(mapper, sessionService, apiService);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(null);

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
            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => apiService.GetAssessment()).Returns(assessmentResponse);

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
            A.CallTo(() => sessionService.GetValue<string>(SessionKey.SessionId)).Returns(sessionId);
            A.CallTo(() => apiService.GetAssessment()).Returns(assessmentResponse);

            var actionResponse = await controller.Index().ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
