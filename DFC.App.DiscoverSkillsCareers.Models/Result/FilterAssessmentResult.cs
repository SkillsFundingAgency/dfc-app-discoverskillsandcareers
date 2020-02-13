using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    public class FilterAssessmentResult
    {
        public FilterAssessmentResult()
        {
            SuggestedJobProfiles = new List<string>();
        }

        public string JobFamilyName { get; set; }

        public DateTime CreatedDt { get; set; }

        public string QuestionSetVersion { get; set; }

        public int RecordedAnswerCount { get; set; }

        public int MaxQuestions { get; set; }

        public IEnumerable<AnswerDetail> RecordedAnswers { get; set; }

        public IEnumerable<string> SuggestedJobProfiles { get; set; }

        public IEnumerable<string> WhatYouToldUs { get; set; }

        public string JobFamilyNameUrlSafe => JobFamilyName?.ToLower()?.Replace(" ", "-");
    }
}
