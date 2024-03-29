﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services.Processors;
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
        public IDocumentStore FakeDocumentStore = A.Fake<IDocumentStore>();

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
            
            A.CallTo(() => FakeMapper.Map<DysacQuestionSetContentModel>(A<ApiQuestionSet>.Ignored)).Returns(questionSetModel);
            A.CallTo(() => FakeMapper.Map<DysacSkillContentModel>(A<ApiSkill>.Ignored)).Returns(BuildSkillModel());
            A.CallTo(() => FakeMapper.Map<DysacTraitContentModel>(A<ApiTrait>.Ignored)).Returns(traitModel);

            A.CallTo(() => FakeCmsApiService.GetItemAsync<ApiQuestionSet>(A<Uri>.Ignored)).Returns(apiQuestionSetModel);
            A.CallTo(() => FakeCmsApiService.GetItemAsync<ApiTrait>(A<Uri>.Ignored)).Returns(BuildApiTraitModel());
            A.CallTo(() => FakeCmsApiService.GetItemAsync<ApiSkill>(A<Uri>.Ignored)).Returns(BuildApiSkillModel());
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
                JobCategories = new List<JobCategoryContentItemModel>
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
                ItemId = Guid.NewGuid(),
                Url = new Uri("http://somewhere.com/somewhereelse/aresource")
            };
        }

        private ApiSkill BuildApiSkillModel()
        {
            return new ApiSkill
            {
                Title = "A skill",
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
                Url = new Uri("http://somewhere.com/somelocation/aresource"),
                ShortQuestions = new List<DysacShortQuestionContentItemModel>
                {
                    new DysacShortQuestionContentItemModel
                    {
                        Impact = "Positive",
                        Title = "A Question",
                        ItemId = QuestionSetItemId,
                        Url = new Uri("http://somewhere.com/somewhereelse/aresource"),
                        Traits = new List<DysacTraitContentItemModel>
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
