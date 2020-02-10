﻿namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    public class GetResultsResponse
    {
        public string SessionId { get; set; }

        public JobCategoryResult[] JobCategories { get; set; }

        public string[] Traits { get; set; }

        public int JobFamilyCount { get; set; }

        public int JobFamilyMoreCount { get; set; }

        public string AssessmentType { get; set; }

        public JobProfileResult[] JobProfiles { get; set; }

        public string[] WhatYouToldUs { get; set; }
    }
}
