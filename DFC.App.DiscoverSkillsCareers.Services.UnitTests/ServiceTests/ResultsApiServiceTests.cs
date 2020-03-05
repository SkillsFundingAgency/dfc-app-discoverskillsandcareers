using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Serialisation;
using FakeItEasy;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ResultsApiServiceTests
    {
        private readonly IResultsApiService resultsApiService;
        private readonly MockHttpMessageHandler httpMessageHandler;
        private readonly HttpClient httpClient;
        private readonly ISerialiser serialiser;
        private readonly IJpOverviewApiService fakeJpOverviewApiService;

        public ResultsApiServiceTests()
        {
            serialiser = new NewtonsoftSerialiser();
            fakeJpOverviewApiService = A.Fake<IJpOverviewApiService>();

            httpMessageHandler = new MockHttpMessageHandler();
            httpClient = httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://localhost/resultsapi");

            resultsApiService = new ResultsApiService(httpClient, serialiser, fakeJpOverviewApiService);
        }

        [Fact]
        public async Task GetResultsReturnsValidDataForSuccess()
        {
            var sessionId = "session1";
            var jobCategory = "short";
            httpMessageHandler.When($"{httpClient.BaseAddress}/result/{sessionId}/{jobCategory}")
                .Respond("application/json", "{'sessionId':'session1'}");

            var resultsResponse = await resultsApiService.GetResults(sessionId);
            Assert.Equal(sessionId, resultsResponse.SessionId);
        }

    }
}
