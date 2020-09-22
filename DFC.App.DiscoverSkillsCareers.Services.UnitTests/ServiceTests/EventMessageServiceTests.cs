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
        private readonly IDocumentServiceFactory fakeDocumentServiceFactory = A.Fake<IDocumentServiceFactory>();
        private readonly IDocumentService<DysacQuestionSetContentModel> fakeDocumentService = A.Fake<IDocumentService<DysacQuestionSetContentModel>>();

        public EventMessageServiceTests()
        {
            A.CallTo(() => fakeDocumentServiceFactory.GetDocumentService<DysacQuestionSetContentModel>()).Returns(fakeDocumentService);
        }

        [Fact]
        public async Task EventMessageServiceGetAllCachedItemsReturnsSuccess()
        {
            // arrange
            var expectedResult = A.CollectionOfFake<DysacQuestionSetContentModel>(2);

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel,bool>>>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.GetAllCachedItemsAsync<DysacQuestionSetContentModel>().ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsSuccess()
        {
            // arrange
            DysacQuestionSetContentModel? existingQuestionSetContentModel = null;
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(existingQuestionSetContentModel);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.CreateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsBadRequestWhenNullSupplied()
        {
            // arrange
            DysacQuestionSetContentModel? DysacQuestionSetContentModel = null;
            var expectedResult = HttpStatusCode.BadRequest;

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.CreateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceCreateAsyncReturnsAlreadyReportedWhenAlreadyExists()
        {
            // arrange
            var existingDysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.AlreadyReported;

            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(existingDysacQuestionSetContentModel);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.CreateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
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

            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(existingDysacQuestionSetContentModel);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(existingDysacQuestionSetContentModel);
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(existingDysacQuestionSetContentModel);
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(false);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(expectedResult);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsBadRequestWhenNullSupplied()
        {
            // arrange
            DysacQuestionSetContentModel? DysacQuestionSetContentModel = null;
            var expectedResult = HttpStatusCode.BadRequest;

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceUpdateAsyncReturnsNotFoundWhenNotExists()
        {
            // arrange
            DysacQuestionSetContentModel? existingDysacQuestionSetContentModel = null;
            var DysacQuestionSetContentModel = A.Fake<DysacQuestionSetContentModel>();
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(existingDysacQuestionSetContentModel);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.UpdateAsync(DysacQuestionSetContentModel).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<DysacQuestionSetContentModel>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceDeleteAsyncReturnsSuccess()
        {
            // arrange
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.DeleteAsync<DysacQuestionSetContentModel>(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task EventMessageServiceDeleteAsyncReturnsNotFound()
        {
            // arrange
            var expectedResult = HttpStatusCode.NotFound;

            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(false);

            var eventMessageService = new EventMessageService(fakeLogger, fakeDocumentServiceFactory);

            // act
            var result = await eventMessageService.DeleteAsync<DysacQuestionSetContentModel>(Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        private DysacQuestionSetContentModel BuildDysacQuestionSetContentModelWithPageLocations()
        {
            var model = new DysacQuestionSetContentModel
            {
                ShortQuestions = new List<IDysacContentModel>
                {
                    new DysacShortQuestion
                    {
                        Title = "A short question",
                        Impact = "Positive",
                        Traits = new List<IDysacContentModel>
                        {
                            new DysacTrait
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
