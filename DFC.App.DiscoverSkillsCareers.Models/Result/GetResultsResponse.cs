using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    public class GetResultsResponse
    {
        public string SessionId { get; set; }

        public IEnumerable<JobCategoryResult> JobCategories { get; set; }

        public IEnumerable<string> Traits { get; set; }

        public int JobFamilyCount { get; set; }

        public int JobFamilyMoreCount { get; set; }

        public string AssessmentType { get; set; }

        public IEnumerable<JobProfileResult> JobProfiles { get; set; }

        public IEnumerable<string> WhatYouToldUs { get; set; }
    }
}
