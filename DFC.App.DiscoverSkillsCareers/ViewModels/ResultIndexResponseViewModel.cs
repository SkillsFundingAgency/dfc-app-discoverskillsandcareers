using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class ResultIndexResponseViewModel
    {
        public string SessionId { get; set; }

        public string Code { get; set; }

        public IEnumerable<JobCategoryResultViewModel> JobCategories { get; set; }

        public IEnumerable<string> Traits { get; set; }

        public int JobFamilyCount { get; set; }

        public int JobFamilyMoreCount { get; set; }

        public string AssessmentType { get; set; }

        public bool UseFilteringQuestions { get; set; }

        public IEnumerable<JobProfileResultViewModel> JobProfiles { get; set; }

        public IEnumerable<string> WhatYouToldUs { get; set; }
    }
}
