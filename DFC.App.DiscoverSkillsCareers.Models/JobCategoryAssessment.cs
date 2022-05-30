using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class JobCategoryAssessment
    {
        public JobCategoryAssessment()
        {
            QuestionSkills = new Dictionary<string, int>();
        }

        private string? jobCategory;

        public string? JobCategory
        {
            get => jobCategory?.Replace(",", string.Empty);
            set => jobCategory = value;
        }

        public Dictionary<string, int> QuestionSkills { get; set; }

        public DateTime LastAnswer { get; set; }
    }
}
