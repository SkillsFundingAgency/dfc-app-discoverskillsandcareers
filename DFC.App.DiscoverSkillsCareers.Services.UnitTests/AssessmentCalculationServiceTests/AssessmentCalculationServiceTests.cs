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
using DFC.Compui.Cosmos;
using FluentAssertions;
using NHibernate.Mapping;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.AssessmentCalculationServiceTests
{
    public class AssessmentCalculationServiceTests
    {
        private readonly IDocumentService<DysacTraitContentModel> traitDocumentService = A.Fake<IDocumentService<DysacTraitContentModel>>();
        private readonly IDocumentService<DysacJobProfileCategoryContentModel> jobProfileCategoryDocumentService = A.Fake<IDocumentService<DysacJobProfileCategoryContentModel>>();
        private readonly IDocumentService<DysacFilteringQuestionContentModel> filteringQuestionDocumentService = A.Fake<IDocumentService<DysacFilteringQuestionContentModel>>();
        private readonly IMapper mapper = A.Fake<IMapper>();

        public AssessmentCalculationServiceTests()
        {
            A.CallTo(() => traitDocumentService.GetAsync(A<Expression<Func<DysacTraitContentModel, bool>>>.Ignored)).Returns(AssessmentHelpers.GetTraits());
            A.CallTo(() => traitDocumentService.GetAllAsync(A<string>.Ignored)).Returns(AssessmentHelpers.GetTraits());
            A.CallTo(() => jobProfileCategoryDocumentService.GetAsync(A<Expression<Func<DysacJobProfileCategoryContentModel, bool>>>.Ignored))
                .Returns(AssessmentHelpers.GetAllJobCategories());
        }

        [Fact]
        public async Task CalculateJobFamilyRelevanceTests()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(
                traitDocumentService,
                jobProfileCategoryDocumentService,
                filteringQuestionDocumentService,
                mapper,
                A.Fake<ILoggerFactory>());

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
            };
            
            // Act
            var result = serviceToTest.CalculateJobFamilyRelevance(
                new List<TraitResult> { new TraitResult { TraitCode = "LEADER" } }, 
                new List<DysacTraitContentModel> { new DysacTraitContentModel { Title = "LEADER", JobCategories = jobCategories } }, 
                new List<DysacFilteringQuestionContentModel> { new DysacFilteringQuestionContentModel { Title = "QUESTION1", Skills = skills } },
                new List<DysacJobProfileCategoryContentModel> { new DysacJobProfileCategoryContentModel { Title = "CATEGORY1", Url = new Uri("http://localhost/category1"), JobProfiles = jobProfiles} });
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task AssessmentCalculationServiceWhenLeaderQuestionPositiveReturnsLeaderJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(
                traitDocumentService,
                jobProfileCategoryDocumentService,
                filteringQuestionDocumentService,
                mapper,
                A.Fake<ILoggerFactory>());
            
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.Questions.FirstOrDefault(x => x.Trait == "LEADER").Answer!.Value = Core.Enums.Answer.StronglyAgree;

            // Act
            var result = await serviceToTest.ProcessAssessment(assessment);

            // Assert
            Assert.Single(result.ShortQuestionResult!.JobCategories);
            Assert.Equal("border-force-leader", result.ShortQuestionResult!.JobCategories.FirstOrDefault().JobFamilyNameUrl);
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenMultipleQuestionPositiveReturnsMultipleJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, jobProfileCategoryDocumentService, filteringQuestionDocumentService, mapper, A.Fake<ILoggerFactory>());
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.Questions.FirstOrDefault(x => x.Trait == "LEADER").Answer!.Value = Core.Enums.Answer.StronglyAgree;
            assessment.Questions.FirstOrDefault(x => x.Trait == "DOER").Answer!.Value = Core.Enums.Answer.StronglyAgree;

            // Act
            var result = await serviceToTest.ProcessAssessment(assessment);

            // Assert
            Assert.Equal(2, result.ShortQuestionResult!.JobCategories.Count());
            Assert.Equal("border-force-leader", result.ShortQuestionResult!.JobCategories.FirstOrDefault().JobFamilyNameUrl);
            Assert.Equal("border-force-doer", result.ShortQuestionResult!.JobCategories.LastOrDefault().JobFamilyNameUrl);
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenAllNegativeReturnsNoJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, jobProfileCategoryDocumentService, filteringQuestionDocumentService, mapper, A.Fake<ILoggerFactory>());
            var assessment = AssessmentHelpers.GetAssessment();

            // Act
            var result = await serviceToTest.ProcessAssessment(assessment);

            // Assert
            Assert.Empty(result.ShortQuestionResult!.JobCategories);
        }

       
    }
}
