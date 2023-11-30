using DFC.App.DiscoverSkillsCareers.Models;
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
    [Trait("Category", "Webhooks Service DeleteContentItemAsync Unit Tests")]
    public class WebhooksServiceDeleteContentItemAsyncTests : BaseWebhooksServiceTests
    {
        /*[Fact]
        public async Task WebhooksServiceDeleteContentItemAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = await service.DeleteContentItemAsync(new DysacQuestionSetContentModel(), ContentIdForDelete, new List<ContentCacheResult>() { new ContentCacheResult() { ContentType = DysacConstants.ContentTypePersonalityQuestionSet, ParentContentId = Guid.NewGuid() } }).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceDeleteContentItemAsyncNotCacheForCreateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentProcessors[0].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.NotFound);

            // Act
            var result = await service.DeleteContentItemAsync(new DysacQuestionSetContentModel(), ContentIdForDelete, new List<ContentCacheResult>()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }*/
    }
}
