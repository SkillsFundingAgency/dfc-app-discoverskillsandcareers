using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Enums;
using DFC.Content.Pkg.Netcore.Data.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessMessageAsync Unit Tests")]
    public class WebhooksServiceProcessMessageAsyncTests : BaseWebhooksServiceTests
    {
        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncForContentCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentCacheService.GetContentCacheStatus(A<Guid>.Ignored)).Returns(new List<ContentCacheResult> { new ContentCacheResult { ContentType = DysacConstants.ContentTypePersonalityQuestionSet, ParentContentId = Guid.NewGuid(), Result = ContentCacheStatus.Content } });

            // Act
            var result = await service.ProcessMessageAsync(Models.Enums.WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, "http://somewhere.com/somewherelese/resource1", DysacConstants.ContentTypePersonalityQuestionSet).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncForContentItemCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentCacheService.GetContentCacheStatus(A<Guid>.Ignored)).Returns(new List<ContentCacheResult> { new ContentCacheResult { ContentType = DysacConstants.ContentTypePersonalityQuestionSet, ParentContentId = Guid.NewGuid(), Result = ContentCacheStatus.ContentItem } });

            // Act
            var result = await service.ProcessMessageAsync(Models.Enums.WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, "http://somewhere.com/somewherelese/resource1", DysacConstants.ContentTypePersonalityQuestionSet).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].ProcessContentItem(A<Guid>.Ignored, A<Guid>.Ignored, A<IBaseContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncForContentDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentCacheService.GetContentCacheStatus(A<Guid>.Ignored)).Returns(new List<ContentCacheResult> { new ContentCacheResult { ContentType = DysacConstants.ContentTypePersonalityQuestionSet, ParentContentId = Guid.NewGuid(), Result = ContentCacheStatus.Content } });

            // Act
            var result = await service.ProcessMessageAsync(Models.Enums.WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForCreate, "http://somewhere.com/somewherelese/resource1", DysacConstants.ContentTypePersonalityQuestionSet).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncForContentItemDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentCacheService.GetContentCacheStatus(A<Guid>.Ignored)).Returns(new List<ContentCacheResult> { new ContentCacheResult { ContentType = DysacConstants.ContentTypePersonalityQuestionSet, ParentContentId = Guid.NewGuid(), Result = ContentCacheStatus.ContentItem } });

            // Act
            var result = await service.ProcessMessageAsync(Models.Enums.WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForCreate, "http://somewhere.com/somewherelese/resource1", DysacConstants.ContentTypePersonalityQuestionSet).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentItemAsync(A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }
    }
}
