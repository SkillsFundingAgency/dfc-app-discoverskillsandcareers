using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ResultsServiceTests
    {
        private readonly ILogger<ResultsService> logger;
        private readonly IResultsApiService resultsApiService;
        private readonly IJpOverviewApiService jPOverviewAPIService;
        private readonly ISessionService  sessionService;
        private readonly IResultsService resultsService;
        private readonly string sessionId;

        public ResultsServiceTests()
        {
            logger = A.Fake<ILogger<ResultsService>>();
            resultsApiService = A.Fake<IResultsApiService>();
            sessionService = A.Fake<ISessionService>();
            jPOverviewAPIService = A.Fake<IJpOverviewApiService>();
            resultsService = new ResultsService(logger, resultsApiService, jPOverviewAPIService, sessionService);

            sessionId = "session1";
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);

        }

        [Fact]
        public async Task GetResults()
        {
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            A.CallTo(() => resultsApiService.GetResults(sessionId, A<string>.Ignored)).Returns(resultsResponse);

            var results = await resultsService.GetResults();

            A.CallTo(() => resultsApiService.GetResults(sessionId, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            results.SessionId.Should().Be(sessionId);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 1)]
        public async Task GetResultsByCategory (bool hasMatchedProfile, int expectedNumberOfcalls)
        {
            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId};

            if (hasMatchedProfile)
            {
                List<JobProfileResult> profiles = new List<JobProfileResult>();
                profiles.Add(new JobProfileResult() { UrlName = "Cname1", JobCategory = category });
                resultsResponse.JobProfiles = profiles;

                List<JobCategoryResult> categories = new List<JobCategoryResult>();
                categories.Add(new JobCategoryResult() { JobFamilyName = category, JobFamilyUrl =category });
                resultsResponse.JobCategories = categories;
            }

            A.CallTo(() => resultsApiService.GetResults(sessionId, A<string>.Ignored)).Returns(resultsResponse);
            A.CallTo(() => jPOverviewAPIService.GetOverviewsForProfilesAsync(A<IEnumerable<string>>.Ignored)).Returns(A.CollectionOfDummy<JobProfileOverView>(2));

            var results = await resultsService.GetResultsByCategory(category);
            
            A.CallTo(() => resultsApiService.GetResults(sessionId, category)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jPOverviewAPIService.GetOverviewsForProfilesAsync(A<IEnumerable<string>>.Ignored)).MustHaveHappened(expectedNumberOfcalls, Times.Exactly);
            results.SessionId.Should().Be(sessionId);
        }
    }
}
