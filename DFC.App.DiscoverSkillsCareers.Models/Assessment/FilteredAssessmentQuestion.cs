using System;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public class FilteredAssessmentQuestion
    {
        public Guid? Id { get; set; }

        public string? QuestionText { get; set; }

        public QuestionAnswer? Answer { get; set; }

        public int? Ordinal { get; set; }

        public string? TraitCode { get; set; }
    }
}
