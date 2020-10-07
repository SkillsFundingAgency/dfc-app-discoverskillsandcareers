using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryResult
    {
        public JobCategoryResult()
        {
            TraitValues = new List<TraitValue>();
        }

        public string JobFamilyCode { get; set; }

        public string JobFamilyName { get; set; }

        public string JobFamilyText { get; set; }

        public string JobFamilyUrl { get; set; }

        public int TraitsTotal { get; set; }

        public decimal Total { get; set; }

        public decimal NormalizedTotal { get; set; }

        public List<TraitValue> TraitValues { get; set; }

        public int TotalQuestions { get; set; }

        public bool ResultsShown { get; set; }

        public string JobFamilyNameUrl => JobFamilyName?.ToLower()?.Replace(" ", "-");

        public int? DisplayOrder { get; set; }

        public int NumberOfMatchedJobProfile { get; set; }

        public IEnumerable<JobProfileOverView> JobProfilesOverviews { get; set; }

        public IEnumerable<JobProfileResult> JobProfiles { get; set; }

        public IEnumerable<string?> SkillQuestions { get; set; }
    }
}