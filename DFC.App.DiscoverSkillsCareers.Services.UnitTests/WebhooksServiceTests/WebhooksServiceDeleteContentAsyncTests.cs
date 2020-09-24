using DFC.App.DiscoverSkillsCareers.Models;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service DeleteContentAsync Unit Tests")]
    public class WebhooksServiceDeleteContentAsyncTests : BaseWebhooksServiceTests
    {
        [Fact]
        public async Task WebhooksServiceDeleteContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = await service.DeleteContentAsync(new DysacQuestionSetContentModel(), ContentIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceDeleteContentAsyncForCreateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NotFound;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.NotFound);

            // Act
            var result = await service.DeleteContentAsync(new DysacQuestionSetContentModel(), ContentIdForDelete).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }
    }
}
