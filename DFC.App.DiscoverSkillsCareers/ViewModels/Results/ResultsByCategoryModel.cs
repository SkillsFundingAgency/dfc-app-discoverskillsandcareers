using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultsByCategoryModel
    {
        public string AssessmentReference { get; set; }

        public Uri ExploreCareersUri { get; set; }

        public string AssessmentType { get; set; }

        public IEnumerable<string> Traits { get; set; }

        public IEnumerable<ResultsJobsInCategoryModel> JobsInCategory { get; set; }
    }
}
