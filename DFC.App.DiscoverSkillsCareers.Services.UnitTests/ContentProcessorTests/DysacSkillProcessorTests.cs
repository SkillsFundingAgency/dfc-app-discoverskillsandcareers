using DFC.App.DiscoverSkillsCareers.Models;
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
    public class DysacSkillProcessorTests : BaseContentProcessorTests
    {
        [Fact]
        public async Task DysacSkillProcessorProcessContentAsyncReturnsSuccess()
        {
            base.Setup();

            //Arrange
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacSkillContentModel>.Ignored)).Returns(HttpStatusCode.OK);
            var processor = new DysacSkillContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentServiceFactory, FakeMappingService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacSkillContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task DysacQuestionSetProcessorProcessContentAsyncCreateNotUpdateReturnsSuccess()
        {
            base.Setup();

            //Arrange
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacSkillContentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<DysacSkillContentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var processor = new DysacSkillContentProcessor(FakeCmsApiService, FakeMapper, FakeEventMessageService, FakeContentCacheService, FakeLogger, FakeDocumentServiceFactory, FakeMappingService);

            //Act
            var result = await processor.ProcessContent(new Uri("http://somewhere.com/somewhereelse/aresource"), Guid.NewGuid());

            //Assert
            A.CallTo(() => FakeEventMessageService.UpdateAsync(A<DysacSkillContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeEventMessageService.CreateAsync(A<DysacSkillContentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeContentCacheService.AddOrReplace(A<Guid>.Ignored, A<List<Guid>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(HttpStatusCode.OK, result);
        }
    }
}
