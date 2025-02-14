﻿using DFC.App.DiscoverSkillsCareers.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultsByCategoryModel
    {
        public string AssessmentReference { get; set; }

        public string AssessmentType { get; set; }

        public IEnumerable<TraitResult> Traits { get; set; }

        public IEnumerable<ResultsJobsInCategoryModel> JobsInCategory { get; set; }

        public string SpeakToAnAdviser { get; set; }
    }
}
