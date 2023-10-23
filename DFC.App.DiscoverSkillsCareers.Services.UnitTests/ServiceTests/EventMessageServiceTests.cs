using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Cosmos.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    [Trait("Category", "Event Message Service Unit Tests")]
    public class EventMessageServiceTests
    {
        private readonly ILogger<EventMessageService> fakeLogger = A.Fake<ILogger<EventMessageService>>();
        private readonly IDocumentStore fakeDocumentStore = A.Fake<IDocumentStore>();

        [Fact]
        public async Task EventMessageServiceGetAllCachedItemsReturnsSuccess()
        {
            // arrange
            var expectedResult = new List<DysacQuestionSetContentModel>
            {
                new DysacQuestionSetContentModel(),
                new DysacQuestionSetContentModel()
            };

            A.CallTo(() => fakeDocumentStore.GetAllContentAsync<DysacQuestionSetContentModel>("QuestionSet", "Test"))
                .Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetAllContentAsync<DysacQuestionSetContentModel>("QuestionSet", "Test")).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsSuccess()
        {
            // arrange
            DysacQuestionSetContentModel? existingQuestionSetContentModel = null;
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test"))
                .Returns(existingQuestionSetContentModel);
            A.CallTo(() => fakeDocumentStore.CreateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test"))
                .Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.CreateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.CreateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsBadRequestWhenNullSupplied()
        {
            // arrange
            DysacQuestionSetContentModel? DysacQuestionSetContentModel = null;
            var expectedResult = HttpStatusCode.BadRequest;

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.CreateAsync(DysacQuestionSetContentModel!).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsAlreadyReportedWhenAlreadyExists()
        {
            // arrange
            var existingDysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.AlreadyReported;

            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test"))
                .Returns(existingDysacQuestionSetContentModel);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.CreateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsSuccessForSamePartitionKey()
        {
            // arrange
            var existingDysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.OK;

            DysacQuestionSetContentModel.PartitionKey = "a-partition-key";
            existingDysacQuestionSetContentModel.PartitionKey = DysacQuestionSetContentModel.PartitionKey;

            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test"))
                .Returns(existingDysacQuestionSetContentModel);
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsSuccessForDifferentPartitionKey()
        {
            // arrange
            var existingDysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.Created;

            DysacQuestionSetContentModel.PartitionKey = "a-partition-key";
            existingDysacQuestionSetContentModel.PartitionKey = "a-different-partition-key";

            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test"))
                .Returns(existingDysacQuestionSetContentModel);
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).Returns(true);
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsSuccessForDifferentPartitionKeyDeleteError()
        {
            // arrange
            var existingDysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.Created;

            DysacQuestionSetContentModel.PartitionKey = "a-partition-key";
            existingDysacQuestionSetContentModel.PartitionKey = "a-different-partition-key";

            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).Returns(existingDysacQuestionSetContentModel);
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).Returns(false);
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsBadRequestWhenNullSupplied()
        {
            // arrange
            DysacQuestionSetContentModel? DysacQuestionSetContentModel = null;
            var expectedResult = HttpStatusCode.BadRequest;

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel!).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsNotFoundWhenNotExists()
        {
            // arrange
            DysacQuestionSetContentModel? existingDysacQuestionSetContentModel = null;
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test"))
                .Returns(existingDysacQuestionSetContentModel);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentStore.UpdateContentAsync(A<DysacQuestionSetContentModel>.Ignored, "Test")).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceDeleteAsyncReturnsSuccess()
        {
            // arrange
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test"))
                .Returns(true);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.DeleteAsync<DysacQuestionSetContentModel>(Guid.NewGuid(), string.Empty).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceDeleteAsyncReturnsNotFound()
        {
            // arrange
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).Returns(false);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentStore);

            // act
            var result = await eventMessageService.DeleteAsync<DysacQuestionSetContentModel>(Guid.NewGuid(), string.Empty).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentStore.DeleteContentAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored, "Test")).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        private DysacQuestionSetContentModel BuildDysacQuestionSetContentModelWithPageLocations()
        {
            var model = new DysacQuestionSetContentModel
            {
                ShortQuestions = new List<DysacShortQuestionContentItemModel>
                {
                    new DysacShortQuestionContentItemModel
                    {
                        Title = "A short question",
                        Impact = "Positive",
                        Traits = new List<DysacTraitContentItemModel>
                        {
                            new DysacTraitContentItemModel
                            {
                                Title = "A trait",
                                Url = new Uri("http://somewhere.com/aplace/aresource"),
                                Description = "The trait description"
                            },
                        },
                    },
                },
            };

            return model;
        }
    }
}
