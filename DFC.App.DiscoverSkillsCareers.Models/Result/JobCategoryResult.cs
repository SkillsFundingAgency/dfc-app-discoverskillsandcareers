using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryResult
    {
        public JobCategoryResult()
        {
            TraitValues = new List<TraitValue>();
            JobProfilesOverviews = new List<JobProfileOverView>();
            JobProfiles = new List<JobProfileResult>();
            SkillQuestions = new List<string>();
        }

        [JsonIgnore]
        public string? JobFamilyCode { get; set; }

        public string? JobFamilyName { get; set; }

        public string? JobFamilyText { get; set; }

        public string? ImagePathDesktop { get; set; }

        public string? ImagePathMobile { get; set; }

        public string? ImagePathTitle { get; set; }

        [JsonIgnore]
        public string? JobFamilyUrl { get; set; }

        public decimal Total { get; set; }

        public List<TraitValue> TraitValues { get; set; }

        public int TotalQuestions { get; set; }

        [JsonIgnore]
        public bool ResultsShown { get; set; }

        public string? JobFamilyNameUrl => JobFamilyName?.ToLower()?.Replace(" ", "-").Replace(",", string.Empty);

        [JsonIgnore]
        public int? DisplayOrder { get; set; }

        [JsonIgnore]
        public IEnumerable<JobProfileOverView> JobProfilesOverviews { get; set; }

        public IEnumerable<JobProfileResult> JobProfiles { get; set; }

        public IEnumerable<string> SkillQuestions { get; set; }
    }
}