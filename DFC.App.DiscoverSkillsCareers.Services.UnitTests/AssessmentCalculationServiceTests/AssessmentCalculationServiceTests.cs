using AutoMapper;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            A.CallTo(() => traitDocumentService.GetAsync(A<Expression<Func<DysacTraitContentModel, bool>>>.Ignored)).Returns(GetTraits());
            A.CallTo(() => traitDocumentService.GetAllAsync(A<string>.Ignored)).Returns(GetTraits());
        }

        [Fact]
        public async Task AssessmentCalculationServiceWhenLeaderQuestionPositiveReturnsLeaderJobCategory()
        {
            // Arrange
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, mapper);
            var assessment = GetAssessment();
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
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, mapper);
            var assessment = GetAssessment();
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
            var serviceToTest = new AssessmentCalculationService(traitDocumentService, mapper);
            var assessment = GetAssessment();

            // Act
            var result = await serviceToTest.ProcessAssessment(assessment);

            // Assert
            Assert.Empty(result.ShortQuestionResult!.JobCategories);
        }

        private DysacAssessment GetAssessment()
        {
            var assessmentToReturn = new DysacAssessment();
            assessmentToReturn.Questions = GetShortQuestions();

            return assessmentToReturn;
        }

        private static List<ShortQuestion> GetShortQuestions()
        {
            return new List<ShortQuestion>
            {
                new ShortQuestion{ Ordinal = 0, Trait = "LEADER", QuestionText = "Am I a leader?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 1, Trait = "ORGANISER", QuestionText = "Am I an organiser?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 2, Trait = "INFLUENCER", QuestionText = "Am I an influencer?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 3, Trait = "DRIVER", QuestionText = "Am I a driver?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 4, Trait = "ANALYST", QuestionText = "Am I an analyst?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 5, Trait = "DOER", QuestionText = "Am I a doer?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 6, Trait = "CREATER", QuestionText = "Am I a creater?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
                new ShortQuestion{ Ordinal = 7, Trait = "HELPER", QuestionText = "Am I a helper?", IsNegative = false, Answer = new QuestionAnswer { AnsweredAt = DateTime.UtcNow, Value = Core.Enums.Answer.Disagree } },
            };
        }

        private IEnumerable<DysacTraitContentModel> GetTraits()
        {
            var listOfTraits = new List<DysacTraitContentModel>();

            listOfTraits.Add(new DysacTraitContentModel { Title = "LEADER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Leader", WebsiteURI = "/job-profiles/border-force-leader" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "ORGANISER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Organiser", WebsiteURI = "/job-profiles/border-force-organiser" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "INFLUENCER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Influencer", WebsiteURI = "/job-profiles/border-force-influencer" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "DRIVER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Driver", WebsiteURI = "/job-profiles/border-force-driver" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "ANALYST", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Analyst", WebsiteURI = "/job-profiles/border-force-analyst" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "DOER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Doer", WebsiteURI = "/job-profiles/border-force-doer" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "CREATER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Creater", WebsiteURI = "/job-profiles/border-force-creater" } } });
            listOfTraits.Add(new DysacTraitContentModel { Title = "HELPER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel>() { new JobCategoryContentItemModel { Title = "Border Force Helper", WebsiteURI = "/job-profiles/border-force-helper" } } });

            return listOfTraits;
        }
    }
}
