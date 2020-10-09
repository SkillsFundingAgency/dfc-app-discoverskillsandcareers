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

        public string? JobCategory { get; set; }

        public Dictionary<string, int> QuestionSkills { get; set; }

        public DateTime LastAnswer { get; set; }
    }
}
