using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class GetQuestionResponse : IQuestion
    {
        public string QuestionText { get; set; }

        public string TraitCode { get; set; }

        public string QuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string SessionId { get; set; }

        public int PercentComplete { get; set; }

        public int? NextQuestionNumber { get; set; }

        public bool IsComplete { get; set; }

        public string ReloadCode { get; set; }

        public DateTime StartedDt { get; set; }

        public int RecordedAnswersCount { get; set; }

        public QuestionAnswer? RecordedAnswer { get; set; }

        public int MaxQuestionsCount { get; set; }

        public string CurrentFilterAssessmentCode { get; set; }

        public bool IsFilterAssessment { get; set; }

        public string JobCategorySafeUrl { get; set; }

        public int CurrentQuestionNumber { get; set; }

        public int? PreviousQuestionNumber { get; set; }
    }
}
