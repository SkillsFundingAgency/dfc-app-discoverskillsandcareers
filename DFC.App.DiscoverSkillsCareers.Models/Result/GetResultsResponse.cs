﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class GetResultsResponse
    {
        public GetResultsResponse()
        {
            JobCategories = new List<JobCategoryResult>();
            JobProfiles = new List<JobProfileResult>();
            WhatYouToldUs = new List<string>();
        }

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
