﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryResultViewModel
    {
        public string JobFamilyCode { get; set; }

        public string JobFamilyName { get; set; }

        public string JobFamilyText { get; set; }

        public string JobFamilyUrl { get; set; }

        public int TraitsTotal { get; set; }

        public decimal Total { get; set; }

        public decimal NormalizedTotal { get; set; }

        public IEnumerable<TraitValueViewModel> TraitValues { get; set; }

        public FilterAssessmentResultViewModel FilterAssessment { get; set; }

        public int TotalQuestions { get; set; }

        public bool ResultsShown { get; set; }

        public string JobFamilyNameUrl { get; set; }
    }
}