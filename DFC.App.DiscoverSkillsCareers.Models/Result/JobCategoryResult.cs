using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryResult
    {
        public string JobFamilyCode { get; set; }

        public string JobFamilyName { get; set; }

        public string JobFamilyText { get; set; }

        public string JobFamilyUrl { get; set; }

        public int TraitsTotal { get; set; }

        public decimal Total { get; set; }

        public decimal NormalizedTotal { get; set; }

        public IEnumerable<TraitValue> TraitValues { get; set; }

        public FilterAssessmentResult FilterAssessment { get; set; }

        public int TotalQuestions { get; set; }

        public bool ResultsShown { get; set; }

        public string JobFamilyNameUrl => JobFamilyName?.ToLower()?.Replace(" ", "-");
    }
}