using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class JobProfileOverviewApiServiceTests
    {
        [Fact]
        public void JobProfileOverviewApiServiceGetOverviewReturnsOverview()
        {
            // Arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(new DysacJobProfileOverviewContentModel { Html = "<h1>A Job Profile</h1>", Id = Guid.NewGuid(), Title = "A Test Job Profile Overview" })) };

            var serviceToTest = new JobProfileOverviewApiService(httpClient, new Models.JobProfileOverviewServiceOptions { BaseAddress = "http://somehwere.com/aresource" });

            // Act
            var result = serviceToTest.GetOverview(new Uri("http://somewhere.com/aresource"));

            // Assert
        }
    }
}
