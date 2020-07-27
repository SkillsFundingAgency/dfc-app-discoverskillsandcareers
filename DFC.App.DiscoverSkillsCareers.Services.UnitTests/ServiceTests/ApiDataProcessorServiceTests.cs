using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.Pages.Data.Contracts;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Pages.Services.ApiProcessorService.UnitTests
{
    [Trait("Category", "API Data Processor Service Unit Tests")]
    public class ApiDataProcessorServiceTests
    {
        private readonly IApiService fakeApiService = A.Fake<IApiService>();

        [Fact]
        public async Task ApiDataProcessorServiceGetReturnsSuccess()
        {
            // arrange
            var expectedResult = new PagesSummaryItemModel
            {
                Url = new Uri("https://somewhere.com"),
                Title = "a-name",
                Published = DateTime.Now,
                CreatedDate = DateTime.Now,
            };
            var jsonResponse = JsonConvert.SerializeObject(expectedResult);

            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).Returns(jsonResponse);

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var result = await apiDataProcessorService.GetAsync<PagesSummaryItemModel>(A.Fake<HttpClient>(), new Uri("https://somewhere.com")).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task ApiDataProcessorServiceGetReturnsNullForNoData()
        {
            // arrange
            PagesSummaryItemModel? expectedResult = null;

            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).Returns(string.Empty);

            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var result = await apiDataProcessorService.GetAsync<PagesSummaryItemModel>(A.Fake<HttpClient>(), new Uri("https://somewhere.com")).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task ApiDataProcessorServiceGetReturnsExceptionForNoHttpClient()
        {
            // arrange
            var apiDataProcessorService = new ApiDataProcessorService(fakeApiService);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await apiDataProcessorService.GetAsync<PagesSummaryItemModel>(null, new Uri("https://somewhere.com")).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeApiService.GetAsync(A<HttpClient>.Ignored, A<Uri>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            Assert.Equal("Value cannot be null. (Parameter 'httpClient')", exceptionResult.Message);
        }
    }
}
