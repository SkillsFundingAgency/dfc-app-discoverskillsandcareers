using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public class DysacAssessment : DocumentModel
    {
        public override string? PartitionKey { get; set; } = "/Assessment";

        public IEnumerable<ShortQuestion> Questions { get; set; }

        public string? AssessmentCode { get; set; }

        public ResultData ShortQuestionResult { get; set; }

        public DateTime StartedAt { get; set; }

        public FilteredAssessment? FilteredAssessment { get; set; }

        [JsonIgnore]
        public AssessmentState AssessmentState => FilteredAssessment == null ? AssessmentState.Short : AssessmentState.Filtered;
    }
}
