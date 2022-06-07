using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;

namespace DFC.App.DiscoverSkillsCareers.Migration.Models
{
    [ExcludeFromCodeCoverage]
    public class DysacAssessmentForCreate
    {
        public DysacAssessmentForCreate()
        {
            Questions = new List<ShortQuestion>();
        }

        public string? PartitionKey { get; set; } = "/Assessment";

        public IEnumerable<ShortQuestion> Questions { get; set; }

        public string? id { get; set; }

        public ResultData? ShortQuestionResult { get; set; }

        public DateTime StartedAt { get; set; }

        public FilteredAssessment? FilteredAssessment { get; set; }

        [JsonIgnore]
        public AssessmentState AssessmentState => FilteredAssessment == null ? AssessmentState.Short : AssessmentState.Filtered;
    }
}
