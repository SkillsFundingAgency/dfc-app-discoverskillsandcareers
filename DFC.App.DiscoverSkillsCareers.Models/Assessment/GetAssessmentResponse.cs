using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class GetAssessmentResponse : IQuestion
    {
        public string? QuestionText { get; set; }

        public string? QuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string? SessionId { get; set; }

        public int PercentComplete { get; set; }

        public int? NextQuestionNumber { get; set; }

        public bool IsComplete
        {
            get { return MaxQuestionsCount == RecordedAnswersCount; }
        }

        public bool IsFilterComplete
        {
            get { return AtLeastOneAnsweredFilterQuestion &&
                    (string.IsNullOrEmpty(CurrentFilterAssessmentCode) || AllFilteringQuestionsForCategoryAnswered != false);
            }
        }

        public string? ReloadCode { get; set; }

        public string? ReferenceCode { get; set; }

        public DateTime StartedDt { get; set; }

        public int RecordedAnswersCount { get; set; }

        public int MaxQuestionsCount { get; set; }

        public string? CurrentFilterAssessmentCode { get; set; }

        public bool IsFilterAssessment { get; set; }

        public string? JobCategorySafeUrl { get; set; }

        public int CurrentQuestionNumber { get; set; }

        public int? PreviousQuestionNumber { get; set; }

        public bool AtLeastOneAnsweredFilterQuestion { get; set; }

        public bool? AllFilteringQuestionsForCategoryAnswered { get; set; }
    }
}
