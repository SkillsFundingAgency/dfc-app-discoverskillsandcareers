using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System;
using Xunit;
using System.Linq;
using DFC.App.DiscoverSkillsCareers.Models.Result;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class JpOverviewApiServiceTests
    {
        private readonly HttpClient httpClient;
        private readonly MockHttpMessageHandler httpMessageHandler;
        private readonly IJpOverviewApiService JpOverviewApiService;
        public JpOverviewApiServiceTests()
        {
            httpMessageHandler = new MockHttpMessageHandler();
            httpClient = httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://localhost");
            JpOverviewApiService = new JpOverviewApiService(httpClient);
        }

        [Fact]
        public async Task GetOverviewsForProfilesAsync()
        {
            var expectedJobProfileOverViews = new List<JobProfileOverView>();
            int numberOfProfiles = 51;

            for (int ii = 0; ii < numberOfProfiles; ii++ )
            {
                expectedJobProfileOverViews.Add(new JobProfileOverView() { Cname = "profile{ii}", OverViewHTML = "<h1>Profile{ii}</h1>" });
            };

            foreach (var profile in expectedJobProfileOverViews)
            {
                httpMessageHandler.When(HttpMethod.Get, $"https://localhost/segment/getbyname/{profile.Cname}").Respond("text/html", profile.OverViewHTML);
            }
            
            var result = await JpOverviewApiService.GetOverviewsForProfilesAsync(expectedJobProfileOverViews.Select(p => p.Cname)).ConfigureAwait(false);

            result.Should().BeEquivalentTo(expectedJobProfileOverViews);
        }

        [Fact]
        public async Task GetOverviewsForProfilesAsyncNullParameters()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => JpOverviewApiService.GetOverviewsForProfilesAsync(null));
        }

        [Fact]
        public async Task GetOverviewsForProfilesThatDoNotExist()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => JpOverviewApiService.GetOverviewsForProfilesAsync(new List<string>() { "DoesNotExit" }));
        }

    }


}
