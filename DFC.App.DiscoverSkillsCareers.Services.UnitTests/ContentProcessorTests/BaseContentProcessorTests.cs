using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ContentProcessorTests
{
    public class BaseContentProcessorTests
    {
        public ILogger<BaseContentProcessor> FakeLogger = A.Fake<ILogger<BaseContentProcessor>>();
        public IDocumentServiceFactory FakeDocumentServiceFactory = A.Fake<IDocumentServiceFactory>();
        public IDocumentService<DysacQuestionSetContentModel> FakeDysacQuestionSetDocumentService = A.Fake<IDocumentService<DysacQuestionSetContentModel>>();
        public IDocumentService<DysacSkillContentModel> FakeDysacSkillDocumentService = A.Fake<IDocumentService<DysacSkillContentModel>>();
        public IDocumentService<DysacTraitContentModel> FakeDysacTraitDocumentService = A.Fake<IDocumentService<DysacTraitContentModel>>();

        public IMappingService FakeMappingService = A.Fake<IMappingService>();
        public IEventMessageService FakeEventMessageService = A.Fake<IEventMessageService>();
        public IContentCacheService FakeContentCacheService = A.Fake<IContentCacheService>();
        public ICmsApiService FakeCmsApiService = A.Fake<ICmsApiService>();
        public IMapper FakeMapper = A.Fake<IMapper>();

        public Guid QuestionSetItemId { get; private set; }
        public Guid QuestionSetId { get; private set; }

        public Guid SkillId { get; private set; }
        public Guid SkillItemId { get; private set; }
        public Guid TraitId { get; private set; }
        public Guid TraitItemId { get; private set; }

        public void Setup()
        {
            var questionSetModel = BuildQuestionSetModel();
            var apiQuestionSetModel = BuildApiQuestionSetModel();

            var traitModel = BuildTraitModel();

            A.CallTo(() => FakeDysacQuestionSetDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(questionSetModel);
            A.CallTo(() => FakeDysacTraitDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(traitModel);

            A.CallTo(() => FakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).Returns(questionSetModel);
            A.CallTo(() => FakeMapper.Map<DysacSkillContentModel>(A<ApiSkill>.Ignored)).Returns(BuildSkillModel());
            A.CallTo(() => FakeMapper.Map<DysacTraitContentModel>(A<ApiTrait>.Ignored)).Returns(traitModel);

            A.CallTo(() => FakeDocumentServiceFactory.GetDocumentService<DysacQuestionSetContentModel>()).Returns(FakeDysacQuestionSetDocumentService);
            A.CallTo(() => FakeDocumentServiceFactory.GetDocumentService<DysacSkillContentModel>()).Returns(FakeDysacSkillDocumentService);
            A.CallTo(() => FakeDocumentServiceFactory.GetDocumentService<DysacTraitContentModel>()).Returns(FakeDysacTraitDocumentService);

            A.CallTo(() => FakeCmsApiService.GetItemAsync<ApiQuestionSet, ApiGenericChild>(A<Uri>.Ignored)).Returns(apiQuestionSetModel);
            A.CallTo(() => FakeCmsApiService.GetItemAsync<ApiTrait, ApiGenericChild>(A<Uri>.Ignored)).Returns(BuildApiTraitModel());
            A.CallTo(() => FakeCmsApiService.GetItemAsync<ApiSkill, ApiGenericChild>(A<Uri>.Ignored)).Returns(BuildApiSkillModel());
        }

        private DysacTraitContentModel BuildTraitModel()
        {
            TraitId = Guid.NewGuid();
            TraitItemId = Guid.NewGuid();

            return new DysacTraitContentModel
            {
                Id = TraitId,
                ItemId = TraitId,
                Description = "A trait description",
                Title = "A trait",
                JobCategories = new List<IDysacContentModel>
                {
                    new JobCategoryContentItemModel
                    {
                        ItemId = TraitItemId,
                        Url = new Uri("http://somewhere.com/somewherelse/alocation"),
                        Description = "A Job Category",
                        Title = "Job category"
                    }
                }
            };
        }

        private ApiQuestionSet BuildApiQuestionSetModel()
        {
            return new ApiQuestionSet
            {
                Type = "Short",
                ItemId = Guid.NewGuid(),
                Url = new Uri("http://somewhere.com/somewhereelse/aresource")
            };
        }

        private ApiSkill BuildApiSkillModel()
        {
            return new ApiSkill
            {
                Title = "A skill",
                Description = "A skill description",
                ItemId = Guid.NewGuid(),
                Url = new Uri("http://somewhere.com/somewhereelse/aresource")
            };
        }

        private ApiTrait BuildApiTraitModel()
        {
            return new ApiTrait
            {
                Title = "A trait",
                ItemId = Guid.NewGuid(),
                Url = new Uri("http://somewhere.com/somewhereelse/aresource")
            };
        }

        public DysacSkillContentModel BuildSkillModel()
        {
            SkillId = Guid.NewGuid();

            return new DysacSkillContentModel
            {
                Title = "A skill",
                ItemId = SkillId,
                Id = SkillId
            };
        }

        public DysacQuestionSetContentModel BuildQuestionSetModel()
        {
            QuestionSetItemId = Guid.NewGuid();
            QuestionSetId = Guid.NewGuid();

            return new DysacQuestionSetContentModel
            {
                Id = QuestionSetId,
                ItemId = QuestionSetId,
                Type = "Short",
                Url = new Uri("http://somewhere.com/somelocation/aresource"),
                ShortQuestions = new List<IDysacContentModel>
                {
                    new DysacShortQuestionContentItemModel
                    {
                        Impact = "Positive",
                        Title = "A Question",
                        ItemId = QuestionSetItemId,
                        Url = new Uri("http://somewhere.com/somewhereelse/aresource"),
                        Traits = new List<IDysacContentModel>
                        {
                            new DysacTraitContentItemModel
                            {
                                ItemId = Guid.NewGuid(),
                                Description = "A Trait",
                                Title = "A trait...",
                                Url = new Uri("http://somewhere.com/somewhereelse/aresource2")
                            }
                        }
                    }
                }
            };
        }
    }
}
