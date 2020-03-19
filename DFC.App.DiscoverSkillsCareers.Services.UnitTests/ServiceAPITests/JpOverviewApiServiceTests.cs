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
using System.Net;

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

        [Theory]
        [InlineData (HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task GetOverviewsForProfilesAsync(HttpStatusCode httpStatusCode)
        {
            var expectedJobProfileOverViews = new List<JobProfileOverView>();
            int numberOfProfiles = 51;

            for (int ii = 0; ii < numberOfProfiles; ii++ )
            {
                expectedJobProfileOverViews.Add(new JobProfileOverView() { Cname = "profile{ii}", OverViewHTML = httpStatusCode == HttpStatusCode.OK ? "<h1>Profile{ii}</h1>" : string.Empty, ReturnedStatusCode = httpStatusCode });
            };

            foreach (var profile in expectedJobProfileOverViews)
            {
                httpMessageHandler.When(HttpMethod.Get, $"https://localhost/segment/getbyname/{profile.Cname}").Respond(profile.ReturnedStatusCode, "text/html", profile.OverViewHTML);
            }
            
            var result = await JpOverviewApiService.GetOverviewsForProfilesAsync(expectedJobProfileOverViews.Select(p => p.Cname)).ConfigureAwait(false);

            result.Should().BeEquivalentTo(expectedJobProfileOverViews);
        }

        [Fact]
        public async Task GetOverviewsForProfilesAsyncNullParameters()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => JpOverviewApiService.GetOverviewsForProfilesAsync(null));
        }
    }
}
