using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class JobProfileOverviewApiServiceTests
    {
        [Fact]
        public async Task JobProfileOverviewApiServiceGetOverviewReturnsOverview()
        {
            //Arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(new DysacJobProfileOverviewContentModel { Html = "<h1>A Job Profile</h1>", Id = Guid.NewGuid(), Title = "A Test Job Profile Overview" })) };

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var serviceToTest = new JobProfileOverviewApiService(httpClient, new Models.JobProfileOverviewServiceOptions { BaseAddress = new Uri("http://somehwere.com/aresource") });

            // Act
            var result = await serviceToTest.GetOverview(new Uri("http://somewhere.com/aresource"));

            // Assert
            Assert.Equal("<h1>A Job Profile</h1>", result.Html);
        }

        [Fact]
        public async Task JobProfileOverviewApiServiceGetOverviewsReturnsOverviews()
        {
            //Arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(new DysacJobProfileOverviewContentModel { Html = "<h1>A Job Profile</h1>", Id = Guid.NewGuid(), Title = "A Test Job Profile Overview" })) };

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var serviceToTest = new JobProfileOverviewApiService(httpClient, new Models.JobProfileOverviewServiceOptions { BaseAddress = new Uri("http://somehwere.com/aresource") });

            // Act
            var result = await serviceToTest.GetOverviews(new List<string> { "a-job-profile-1", "a-job-profile-2", "a-job-profile-3" });

            // Assert
            Assert.Equal(3, result.Count);
        }
    }
}
