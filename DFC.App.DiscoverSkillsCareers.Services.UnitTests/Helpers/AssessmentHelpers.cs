using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.Helpers
{
    public static class AssessmentHelpers
    {
        public static DysacAssessment GetAssessment()
        {
            var assessmentToReturn = new DysacAssessment
            {
                Questions = GetShortQuestions(),
                PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            return assessmentToReturn;
        }

        public static List<ShortQuestion> GetShortQuestions()
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

        public static async Task<List<DysacJobProfileCategoryContentModel>> GetAllJobCategories()
        {
            var listOfCategories = new List<DysacJobProfileCategoryContentModel>
            {
                new DysacJobProfileCategoryContentModel { Title = "border force leader", Url = new Uri("https://localhost/jobprofile/1") },
                new DysacJobProfileCategoryContentModel { Title = "border force doer", Url = new Uri("https://localhost/jobprofile/2") },
            };
            
            return listOfCategories;
        }

        public static async Task<List<DysacTraitContentModel>> GetTraits()
        {
            var listOfTraits = new List<DysacTraitContentModel>
            {
                new DysacTraitContentModel { Title = "LEADER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Leader", Url = new Uri("https://localhost/jobprofile/1"), WebsiteURI = "/job-profiles/border-force-leader" } } },
                new DysacTraitContentModel { Title = "ORGANISER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Organiser", WebsiteURI = "/job-profiles/border-force-organiser" } } },
                new DysacTraitContentModel { Title = "INFLUENCER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Influencer", WebsiteURI = "/job-profiles/border-force-influencer" } } },
                new DysacTraitContentModel { Title = "DRIVER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Driver", WebsiteURI = "/job-profiles/border-force-driver" } } },
                new DysacTraitContentModel { Title = "ANALYST", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Analyst", WebsiteURI = "/job-profiles/border-force-analyst" } } },
                new DysacTraitContentModel { Title = "DOER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Doer", Url = new Uri("https://localhost/jobprofile/2"), WebsiteURI = "/job-profiles/border-force-doer" } } },
                new DysacTraitContentModel { Title = "CREATER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Creater", WebsiteURI = "/job-profiles/border-force-creater" } } },
                new DysacTraitContentModel { Title = "HELPER", Id = Guid.NewGuid(), JobCategories = new List<JobCategoryContentItemModel> { new JobCategoryContentItemModel { Title = "Border Force Helper", WebsiteURI = "/job-profiles/border-force-helper" } } }
            };

            return listOfTraits;
        }
    }
}
