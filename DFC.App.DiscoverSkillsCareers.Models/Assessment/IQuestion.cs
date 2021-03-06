﻿namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public interface IQuestion
    {
        string? QuestionId { get; set; }

        int CurrentQuestionNumber { get; set; }

        int? PreviousQuestionNumber { get; set; }

        int? NextQuestionNumber { get; set; }

        int MaxQuestionsCount { get; set; }
    }
}
