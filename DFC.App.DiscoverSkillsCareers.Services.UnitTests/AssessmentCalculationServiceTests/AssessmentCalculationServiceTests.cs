using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.Helpers;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.AssessmentCalculationServiceTests
{
    public class AssessmentCalculationServiceTests
    {
        private readonly IDocumentService<DysacTraitContentModel> traitDocumentService = A.Fake<IDocumentService<DysacTraitContentModel>>();
        private readonly IMapper mapper = A.Fake<IMapper>();

        public AssessmentCalculationServiceTests()
        {
            A.CallTo(() => traitDocumentService.GetAsync(A<Expression<Func<DysacTraitContentModel, bool>>>.Ignored)).Returns(AssessmentHelpers.GetTraits());
            A.CallTo(() => traitDocumentService.GetAllAsync(A<string>.Ignored)).Returns(AssessmentHelpers.GetTraits());
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenLeaderQuestionPositiveReturnsLeaderJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, mapper, A.Fake<ILogger>());
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
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, mapper, A.Fake<ILogger>());
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
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, mapper, A.Fake<ILogger>());
            var assessment = AssessmentHelpers.GetAssessment();

            // Act
            var result = await serviceToTest.ProcessAssessment(assessment);

            // Assert
            Assert.Empty(result.ShortQuestionResult!.JobCategories);
        }

       
    }
}
