using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public class DysacAssessment : DocumentModel
    {
        public DysacAssessment()
        {
            this.FilteredAssessments = new List<FilteredAssessment>();
        }

        public override string? PartitionKey { get; set; } = "/Assessment";

        public IEnumerable<ShortQuestion> Questions { get; set; }

        public string? AssessmentCode { get; set; }

        public ResultData ShortQuestionResult { get; set; }

        public DateTime StartedAt { get; set; }

        public List<FilteredAssessment> FilteredAssessments { get; set; }
    }
}
