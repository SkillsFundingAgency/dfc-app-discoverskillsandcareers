using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class QuestionSet
    {
        public string PartitionKey { get; set; }

        [JsonProperty("id")]
        public string QuestionSetVersion { get; set; }

        public int Version { get; set; }

        public string AssessmentType { get; set; }

        public int MaxQuestions { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string TitleLowercase { get; set; }

        public bool IsCurrent { get; set; }
    }
}
