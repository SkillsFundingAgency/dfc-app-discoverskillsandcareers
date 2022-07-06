using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ContentProcessorTests
{
    public class JobProfileOverviewContentProcessorTests : BaseContentProcessorTests
    {
        private readonly IDocumentStore fakeDocumentStore;
        private readonly IJobProfileOverviewApiService fakeJobProfileApiOverviewService;

        public JobProfileOverviewContentProcessorTests()
        {
            fakeDocumentStore = A.Fake<IDocumentStore>();
            fakeJobProfileApiOverviewService = A.Fake<IJobProfileOverviewApiService>();
        }

        [Fact]
        public async Task JobProfileOverviewProcessorProcessContentAsyncNonExistingReturnsSuccess()
        {
            DysacJobProfileOverviewContentModel? apiModel = null;

            //Arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(new DysacJobProfileOverviewContentModel { Html = "<h1>A Job Profile</h1>", Id = Guid.NewGuid(), Title = "A Test Job Profile Overview" })) };

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacJobProfileOverviewContentModel>(A<Guid>.Ignored, A<string>.Ignored)).Returns(apiModel);
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentStore);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeJobProfileApiOverviewService.GetOverview(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorProcessContentAsyncExistingReturnsSuccess()
        {
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(new DysacJobProfileOverviewContentModel { Html = "<h1>A Job Profile</h1>", Id = Guid.NewGuid(), Title = "A Test Job Profile Overview" })) };
            DysacJobProfileOverviewContentModel? apiModel = new DysacJobProfileOverviewContentModel() { Id = Guid.NewGuid(), Html = "<h3>Test</h3>" };

            //Arrange
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacJobProfileOverviewContentModel>(A<Guid>.Ignored, A<string>.Ignored)).Returns(apiModel);
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentStore);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeJobProfileApiOverviewService.GetOverview(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorProcessContentItemAsyncCreateReturnsBadRequest()
        {
            //Arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentStore);

            //Act
            var result = await processor.ProcessContentItem(Guid.NewGuid(), Guid.NewGuid(), new ApiJobProfileOverview()).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, result);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorRemoveContentAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentStore);
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacJobProfileOverviewContentModel>(A<Guid>.Ignored, A<string>.Ignored)).Returns(true);

            //Act
            var result = await processor.DeleteContentAsync(Guid.NewGuid(), string.Empty);

            //Assert
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacJobProfileOverviewContentModel>(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorRemoveContentItemAsyncCreateReturnsOk()
        {
            //Arrange
            var processor = new DysacSkillContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            //Act
            //Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await processor.DeleteContentItemAsync(Guid.NewGuid(), Guid.NewGuid(), string.Empty).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}
