﻿using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ContentProcessorTests
{
    [Trait("Category", "Content Processor Unit Tests")]
    public class DysacQuestionSetProcessorTests : BaseContentProcessorTests
    {
        [Fact]
        public async Task DysacQuestionSetProcessorProcessContentAsyncReturnsSuccess()
        {
            base.Setup();

            //Arrange
            A.CallTo(() => FakeEventMessageService.UpdateAsync<DysacQuestionSetContentModel>(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new DysacQuestionSetContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacQuestionSetProcessorProcessContentAsyncCreateNotUpdateReturnsSuccess()
        {
            base.Setup();

            //Arrange
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var processor = new DysacQuestionSetContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacQuestionSetProcessorProcessContentItemAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new DysacQuestionSetContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);

            A.CallTo(() => FakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored))
                .Returns(new DysacQuestionSetContentModel
                {
                    ShortQuestions = new List<DysacShortQuestionContentItemModel>
                    {
                        new DysacShortQuestionContentItemModel
                        {
                            Title = "Question1",
                            ItemId = QuestionSetItemId
                        }
                    }
                });
            
            //Act
            var result = await processor.ProcessContentItem(QuestionSetId, QuestionSetItemId, new ApiShortQuestion());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappened();
            A.CallTo(() => FakeDocumentStore.GetContentByIdAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, A<string>.Ignored))
                .MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacQuestionSetProcessorRemoveContentAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new DysacQuestionSetContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);
            A.CallTo(() => FakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, string.Empty)).Returns(HttpStatusCode.OK);

            //Act
            var result = await processor.DeleteContentAsync(QuestionSetId, string.Empty);

            //Assert
            A.CallTo(() => FakeEventMessageService.DeleteAsync<DysacQuestionSetContentModel>(A<Guid>.Ignored, string.Empty)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacQuestionSetProcessorRemoveContentItemAsyncCreateReturnsOk()
        {
            base.Setup();

            //Arrange
            var processor = new DysacQuestionSetContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentStore, FakeMappingService);
            A.CallTo(() => FakeEventMessageService.UpdateAsync<DysacQuestionSetContentModel>(A<DysacQuestionSetContentModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            var result = await processor.DeleteContentItemAsync(QuestionSetId, QuestionSetItemId, "PartitionKey");

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync<DysacQuestionSetContentModel>(A<DysacQuestionSetContentModel>.Ignored)).MustHaveHappened();

            Assert.Equal(HttpStatusCode.OK, result);
        }
    }
}
