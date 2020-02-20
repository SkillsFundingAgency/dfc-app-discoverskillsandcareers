using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
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
