using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.Helpers;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.AssessmentCalculationServiceTests
{
    public class AssessmentCalculationServiceTests
    {
        private readonly IDocumentStore documentStore = A.Fake<IDocumentStore>();
        private readonly AssessmentService assessmentService = A.Fake<AssessmentService>();
        private readonly IMapper mapper = A.Fake<IMapper>();
        private readonly IMemoryCache memoryCache = A.Fake<IMemoryCache>();
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();

        public AssessmentCalculationServiceTests()
        {
            A.CallTo(() => documentStore.GetAllContentAsync<DysacTraitContentModel>("Trait", A<string>.Ignored))
                .Returns(AssessmentHelpers.GetTraits());

            A.CallTo(() => documentStore.GetAllContentAsync<DysacJobProfileCategoryContentModel>("JobProfileCategory", A<string>.Ignored))
                .Returns(AssessmentHelpers.GetAllJobCategories());
        }

        [Fact]
        public async Task CalculateJobFamilyRelevanceTests()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(
                documentStore,
                assessmentService,
                memoryCache,
                mapper,
                A.Fake<ILoggerFactory>(),
                fakeSharedContentRedisInterface);

            var skills = new List<DysacSkillContentItemModel>
            {
                new DysacSkillContentItemModel
                {
                    Title = "SKILL1",
                    ItemId = new Guid(),
                }
            };

            var jobProfiles = new List<JobProfileContentItemModel>
            {
                new JobProfileContentItemModel
                {
                    Title = "JOB1",
                    Skills = skills,
                }
            };

            var jobCategories = new List<JobCategoryContentItemModel>
            {
                new JobCategoryContentItemModel
                {
                    Title = "CATEGORY1",
                    Url = new Uri("http://localhost/category1"),
                    JobProfiles = jobProfiles,
                },
                new JobCategoryContentItemModel
                {
                    Title = "CATEGORY1",
                    Url = new Uri("http://localhost/category1"),
                    JobProfiles = jobProfiles,
                },
            };

            // Act
            var result = serviceToTest.CalculateJobFamilyRelevance(
                new List<TraitResult> { new TraitResult { TraitCode = "LEADER", TotalScore = 1 } },
                new List<DysacTraitContentModel> { new DysacTraitContentModel { Title = "LEADER", JobCategories = jobCategories } },
                new List<DysacFilteringQuestionContentModel> { new DysacFilteringQuestionContentModel { Title = "QUESTION1", Skills = skills } },
                new List<DysacJobProfileCategoryContentModel>
                {
                    new DysacJobProfileCategoryContentModel { Title = "CATEGORY1", Url = new Uri("http://localhost/category1"), JobProfiles = jobProfiles },
                    new DysacJobProfileCategoryContentModel { Title = "CATEGORY1", Url = new Uri("http://localhost/category1"), JobProfiles = jobProfiles }
                });

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenLeaderQuestionPositiveReturnsLeaderJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(
                documentStore,
                assessmentService,
                memoryCache,
                mapper,
                A.Fake<ILoggerFactory>(),
                fakeSharedContentRedisInterface);

            var assessment = AssessmentHelpers.GetAssessment();
            assessment.Questions.FirstOrDefault(x => x.Trait == "LEADER").Answer!.Value = Core.Enums.Answer.StronglyAgree;

            // Act
            var result = await serviceToTest.RunShortAssessmentCalculation(assessment, await AssessmentHelpers.GetTraits());

            // Assert
            Assert.Single(result.ShortQuestionResult!.JobCategories);
            Assert.Equal("border-force-leader", result.ShortQuestionResult!.JobCategories.FirstOrDefault().JobFamilyNameUrl);
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenMultipleQuestionPositiveReturnsMultipleJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(
                documentStore,
                assessmentService,
                memoryCache,
                mapper,
                A.Fake<ILoggerFactory>(),
                fakeSharedContentRedisInterface
                );

            var assessment = AssessmentHelpers.GetAssessment();

            assessment.Questions.FirstOrDefault(x => x.Trait == "LEADER").Answer!.Value = Core.Enums.Answer.StronglyAgree;
            assessment.Questions.FirstOrDefault(x => x.Trait == "DOER").Answer!.Value = Core.Enums.Answer.StronglyAgree;

            // Act
            var result = await serviceToTest.RunShortAssessmentCalculation(assessment, await AssessmentHelpers.GetTraits());

            // Assert
            Assert.Equal(2, result.ShortQuestionResult!.JobCategories.Count());
            Assert.Equal("border-force-leader", result.ShortQuestionResult!.JobCategories.FirstOrDefault().JobFamilyNameUrl);
            Assert.Equal("border-force-doer", result.ShortQuestionResult!.JobCategories.LastOrDefault().JobFamilyNameUrl);
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenAllNegativeReturnsNoJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(
                documentStore,
                assessmentService,
                memoryCache,
                mapper,
                A.Fake<ILoggerFactory>(),
                fakeSharedContentRedisInterface);

            var assessment = AssessmentHelpers.GetAssessment();

            // Act
            var result = await serviceToTest.RunShortAssessmentCalculation(assessment, await AssessmentHelpers.GetTraits());

            // Assert
            Assert.Empty(result.ShortQuestionResult!.JobCategories);
        }
    }
}
