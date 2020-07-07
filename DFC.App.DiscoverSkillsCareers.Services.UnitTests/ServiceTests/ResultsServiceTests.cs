using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Data;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ResultsServiceTests
    {
        private readonly IResultsApiService resultsApiService;
        private readonly IJpOverviewApiService jPOverviewAPIService;
        private readonly IResultsService resultsService;
        private readonly Guid sessionId;

        public ResultsServiceTests()
        {
            resultsApiService = A.Fake<IResultsApiService>();
            FakeSessionStateService = A.Fake<ISessionStateService<SessionDataModel>>();
            Logger = A.Fake<ILogger<ResultsService>>();
            jPOverviewAPIService = A.Fake<IJpOverviewApiService>();
            resultsService = new ResultsService(Logger, resultsApiService, jPOverviewAPIService, FakeSessionStateService);

            sessionId = new Guid();
            A.CallTo(() => FakeSessionStateService.GetSessionId()).Returns(sessionId);
        }

        protected ILogger<ResultsService> Logger { get; }

        protected ISessionStateService<SessionDataModel> FakeSessionStateService { get; }

        [Fact]
        public async Task GetResults()
        {
            //Arrange
            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            List<JobProfileResult> profiles = new List<JobProfileResult>
            {
                    new JobProfileResult() { UrlName = category, JobCategory = category }
            };
            resultsResponse.JobProfiles = profiles;

            List<JobCategoryResult> categories = new List<JobCategoryResult>
             {
                    new JobCategoryResult() { JobFamilyName = category, JobFamilyUrl = category }
            };
            resultsResponse.JobCategories = categories;

            A.CallTo(() => resultsApiService.GetResults(sessionId.ToString(), A<string>.Ignored)).Returns(resultsResponse);

            //Act
            var results = await resultsService.GetResults(sessionId);

            //Assert
            A.CallTo(() => resultsApiService.GetResults(sessionId.ToString(), A<string>.Ignored)).MustHaveHappenedOnceExactly();
            results.SessionId.Should().Be(sessionId);
            results.JobCategories.FirstOrDefault().NumberOfMatchedJobProfile.Should().Be(1);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 1)]
        public async Task GetResultsByCategory (bool hasMatchedProfile, int expectedNumberOfcalls)
        {
            //Arrange
            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId.ToString()};

            if (hasMatchedProfile)
            {
                List<JobProfileResult> profiles = new List<JobProfileResult>
                {
                    new JobProfileResult() { UrlName = "Cname1", JobCategory = category }
                };
                resultsResponse.JobProfiles = profiles;

                List<JobCategoryResult> categories = new List<JobCategoryResult>
                {
                    new JobCategoryResult() { JobFamilyName = category, JobFamilyUrl = category }
                };
                resultsResponse.JobCategories = categories;
            }

            A.CallTo(() => resultsApiService.GetResults(sessionId.ToString(), A<string>.Ignored)).Returns(resultsResponse);
            A.CallTo(() => jPOverviewAPIService.GetOverviewsForProfilesAsync(A<IEnumerable<string>>.Ignored)).Returns(A.CollectionOfDummy<JobProfileOverView>(2));

            //Act
            var results = await resultsService.GetResultsByCategory(category, sessionId);
            
            //Assert
            A.CallTo(() => resultsApiService.GetResults(sessionId.ToString(), category)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jPOverviewAPIService.GetOverviewsForProfilesAsync(A<IEnumerable<string>>.Ignored)).MustHaveHappened(expectedNumberOfcalls, Times.Exactly);
            results.SessionId.Should().Be(sessionId.ToString());
        }

    }
}
