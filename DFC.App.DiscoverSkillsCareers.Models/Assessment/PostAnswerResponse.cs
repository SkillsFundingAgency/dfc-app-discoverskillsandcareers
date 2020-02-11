﻿namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    public class PostAnswerResponse
    {
        public bool IsSuccess { get; set; }

        public bool IsComplete { get; set; }

        public string JobCategorySafeUrl { get; set; }

        public bool IsFilterAssessment { get; set; }

        public int NextQuestionNumber { get; set; }
    }
}