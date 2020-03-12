using Dfc.Session;
using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ResultsServiceTests
    {
        private readonly ILogger<ResultsService> logger;
        private readonly IResultsApiService resultsApiService;
        private readonly ISession  session;
        private readonly IResultsService resultsService;

        public ResultsServiceTests()
        {
            logger = A.Fake<ILogger<ResultsService>>();
            resultsApiService = A.Fake<IResultsApiService>();
            session = A.Fake<ISession>();
            resultsService = new ResultsService(logger, resultsApiService, session);
        }

        [Fact]
        public async Task GetResultsCallsGetResultsForCurrentSession()
        {
            var sessionId = "session1";
            var resultsResponse = new GetResultsResponse();
            A.CallTo(() => session.GetSessionId()).Returns(sessionId);
            A.CallTo(() => resultsApiService.GetResults(sessionId)).Returns(resultsResponse);

            await resultsService.GetResults();
            A.CallTo(() => resultsApiService.GetResults(sessionId)).MustHaveHappenedOnceExactly();
        }
    }
}
