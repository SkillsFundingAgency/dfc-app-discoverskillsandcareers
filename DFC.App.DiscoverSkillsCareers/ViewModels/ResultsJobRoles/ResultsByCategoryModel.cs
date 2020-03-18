using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class ResultsByCategoryModel
    {
        public string AssessmentReference { get; set; }

        public Uri ExploreCareersUri { get; set; }

        public IEnumerable<string> Traits { get; set; }

        public IEnumerable<ResultsJobsInCategoryModel> JobsInCategory { get; set; }
    }
}
