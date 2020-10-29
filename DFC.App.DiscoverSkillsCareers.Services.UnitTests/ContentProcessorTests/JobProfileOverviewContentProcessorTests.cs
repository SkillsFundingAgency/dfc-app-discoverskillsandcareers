using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.FakeHttpHandlers;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ContentProcessorTests
{
    public class JobProfileOverviewContentProcessorTests : BaseContentProcessorTests
    {
        private readonly IDocumentService<DysacJobProfileOverviewContentModel> fakeDocumentService;
        private readonly IJobProfileOverviewApiService fakeJobProfileApiOverviewService;

        public JobProfileOverviewContentProcessorTests()
        {
            fakeDocumentService = A.Fake<IDocumentService<DysacJobProfileOverviewContentModel>>();
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
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(apiModel);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).MustHaveHappenedOnceExactly();
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
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(apiModel);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacJobProfileOverviewContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeJobProfileApiOverviewService.GetOverview(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorProcessContentItemAsyncCreateReturnsOk()
        {
            //Arrange
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentService);

            //Act
            //Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await processor.ProcessContentItem(Guid.NewGuid(), Guid.NewGuid(), new ApiJobProfileOverview()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorRemoveContentAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new JobProfileOverviewContentProcessor(fakeJobProfileApiOverviewService, fakeDocumentService);
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            //Act
            var result = await processor.DeleteContentAsync(Guid.NewGuid());

            //Assert
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task JobProfileOverviewProcessorRemoveContentItemAsyncCreateReturnsOk()
        {
            //Arrange
            var processor = new DysacSkillContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentServiceFactory, FakeMappingService);

            //Act
            //Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await processor.DeleteContentItemAsync(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}
