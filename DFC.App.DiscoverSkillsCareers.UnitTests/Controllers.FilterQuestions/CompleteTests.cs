using AutoMapper;
using Dfc.Session;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.FilterQuestions
{
    public class CompleteTests
    {
        private readonly FilterQuestionsController controller;
        private readonly IMapper mapper;
        private readonly ISessionClient sessionClient;
        private readonly IApiService apiService;

        public CompleteTests()
        {
            mapper = A.Fake<IMapper>();
            sessionClient = A.Fake<ISessionClient>();
            apiService = A.Fake<IApiService>();

            controller = new FilterQuestionsController(mapper, sessionClient, apiService);
        }

        [Fact]
        public async Task WhenNoSessionIdRedirectsToRoot()
        {
            string sessionId = null;
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => sessionClient.TryFindSessionCode()).Returns(sessionId);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(actionResponse);
            var redirectResult = actionResponse as RedirectResult;
            Assert.Equal($"~/{RouteName.Prefix}/", redirectResult.Url);
        }

        [Fact]
        public async Task WhenSessionIdExistsReturnsView()
        {
            var sessionId = "sessionId1";
            var viewModel = new FilterQuestionsCompleteResponseViewModel();
            A.CallTo(() => sessionClient.TryFindSessionCode()).Returns(sessionId);

            var actionResponse = await controller.Complete(viewModel).ConfigureAwait(false);

            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
