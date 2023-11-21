using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ContentProcessorTests
{
    [Trait("Category", "Content Processor Unit Tests")]
    public class DysacTraitProcessorTests : BaseContentProcessorTests
    {
        [Fact]
        public async Task DysacTraitProcessorProcessContentAsyncReturnsSuccess()
        {
            base.Setup();

            //Arrange
            A.CallTo(() => FakeEventMessageService.UpdateAsync<DysacTraitContentModel>(A<DysacTraitContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new DysacTraitContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacTraitContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacTraitProcessorProcessContentAsyncCreateNotUpdateReturnsSuccess()
        {
            base.Setup();

            //Arrange
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacTraitContentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<DysacTraitContentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var processor = new DysacTraitContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacTraitContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<DysacTraitContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacTraitProcessorProcessContentItemAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new DysacTraitContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            A.CallTo(() => FakeDocumentStore.GetContentByIdAsync<DysacTraitContentModel>(A<Guid>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new DysacTraitContentModel
                {
                    JobCategories = new List<JobCategoryContentItemModel>
                    {
                        new JobCategoryContentItemModel
                        {
                            ItemId = TraitItemId
                        }
                    }
                });
            
            //Act
            var result = await processor.ProcessContentItem(TraitId, TraitItemId, new ApiTrait());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacTraitContentModel>.Ignored)).MustHaveHappened();
            A.CallTo(() => FakeDocumentStore.GetContentByIdAsync<DysacTraitContentModel>(A<Guid>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacTraitProcessorRemoveContentAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new DysacTraitContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);
            A.CallTo(() => FakeEventMessageService.DeleteAsync<DysacTraitContentModel>(A<Guid>.Ignored, string.Empty)).Returns(HttpStatusCode.OK);

            //Act
            var result = await processor.DeleteContentAsync(TraitId, string.Empty);

            //Assert
            A.CallTo(() => FakeEventMessageService.DeleteAsync<DysacTraitContentModel>(A<Guid>.Ignored, string.Empty)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacTraitProcessorRemoveContentItemAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new DysacTraitContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);
            A.CallTo(() => FakeEventMessageService.UpdateAsync<DysacTraitContentModel>(A<DysacTraitContentModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            var result = await processor.DeleteContentItemAsync(TraitId, TraitItemId, "PartitionKey");

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync<DysacTraitContentModel>(A<DysacTraitContentModel>.Ignored)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }
    }
}
