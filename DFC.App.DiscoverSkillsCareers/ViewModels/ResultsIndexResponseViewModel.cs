using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class ResultsIndexResponseViewModel
    {
        public IEnumerable<JobCategoryResultViewModel> JobCategories { get; set; }

        public IEnumerable<string> Traits { get; set; }

        public int JobFamilyCount { get; set; }

        public int JobFamilyMoreCount { get; set; }

        public string AssessmentType { get; set; }

        public IEnumerable<JobProfileResultViewModel> JobProfiles { get; set; }

        public IEnumerable<string> WhatYouToldUs { get; set; }
    }
}
