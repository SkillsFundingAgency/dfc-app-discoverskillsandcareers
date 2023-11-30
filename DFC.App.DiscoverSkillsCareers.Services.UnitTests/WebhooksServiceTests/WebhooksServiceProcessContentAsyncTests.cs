using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessContentAsync Unit Tests")]
    public class WebhooksServiceProcessContentAsyncTests : BaseWebhooksServiceTests
    {
        /*[Fact]
        public async Task WebhooksServiceProcessContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessContentAsync(new ApiQuestionSet(), new DysacQuestionSetContentModel(), new Uri("http://somewhere.com/someplace"), ContentIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceDeleteContentAsyncForCreateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NotFound;
            var expectedValidContentPageModel = BuildValidContentPageModel();
            var service = BuildWebhooksService();
            A.CallTo(() => FakeContentProcessors[0].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).Returns(HttpStatusCode.NotFound);

            // Assert

            // Act
            var result = await service.ProcessContentAsync(new ApiQuestionSet(), new DysacQuestionSetContentModel(), new Uri("http://somewhere.com/someplace"), ContentIdForCreate).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeContentProcessors[0].ProcessContent(A<Uri>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }*/
    }
}
