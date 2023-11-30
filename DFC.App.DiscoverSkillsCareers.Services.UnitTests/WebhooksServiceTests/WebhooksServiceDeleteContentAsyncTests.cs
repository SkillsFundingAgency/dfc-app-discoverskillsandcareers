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
        /*[Fact]*/
        /*public async Task WebhooksServiceDeleteContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = await service.DeleteContentAsync(new DysacQuestionSetContentModel(), ContentIdForDelete, string.Empty).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored, string.Empty)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceDeleteContentAsyncForCreateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NotFound;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored, string.Empty)).Returns(HttpStatusCode.NotFound);

            // Assert

            // Act
            var result = await service.DeleteContentAsync(new DysacQuestionSetContentModel(), ContentIdForDelete, string.Empty).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].DeleteContentAsync(A<Guid>.Ignored, string.Empty)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }*/
    }
}
