﻿using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class QuestionGetResponseViewModel
    {
        public string QuestionText { get; set; }

        public string TraitCode { get; set; }

        public string QuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string SessionId { get; set; }

        public int PercentageComplete { get; set; }

        public int? NextQuestionNumber { get; set; }

        public bool IsComplete { get; set; }

        public string ReloadCode { get; set; }

        public DateTime Started { get; set; }

        public int RecordedAnswersCount { get; set; }

        [Required(ErrorMessage = "Choose an answer to the statement")]
        public Answer? Answer { get; set; }

        public int AvailableQuestionsCount { get; set; }

        public string CurrentFilterAssessmentCode { get; set; }

        public bool IsFilterAssessment { get; set; }

        public int CurrentQuestionNumber { get; set; }

        public string QuestionSetName { get; set; }

        public string QuestionSetVersion { get; set; }

        public int? PreviousQuestionNumber { get; set; }

        public bool IsChecked(Answer answer)
        {
            return Answer == answer;
        }
    }
}
