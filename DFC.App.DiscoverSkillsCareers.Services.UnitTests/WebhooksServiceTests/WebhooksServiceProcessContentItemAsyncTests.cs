using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.Content.Pkg.Netcore.Data.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessContentItemAsync Unit Tests")]
    public class WebhooksServiceProcessContentItemAsyncTests : BaseWebhooksServiceTests
    {
        [Fact]
        public async Task WebhooksServiceProcessContentItemAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessContentItemAsync(new DysacQuestionSetContentModel(), new Uri("http://somewhere.com/someplace"), ContentItemIdForCreate, new List<ContentCacheResult>() { new ContentCacheResult() { ContentType = DysacConstants.ContentTypePersonalityQuestionSet, ParentContentId = Guid.NewGuid() } }).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<ApiGenericChild>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceDeleteContentItemAsyncForCreateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentProcessors[0].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<ApiGenericChild>.Ignored)).Returns(HttpStatusCode.NotFound);

            // Assert

            // Act
            var result = await service.ProcessContentItemAsync(new DysacQuestionSetContentModel(), new Uri("http://somewhere.com/someplace"), Guid.NewGuid(), new List<ContentCacheResult>()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<ApiGenericChild>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}
