using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class FilterAssessmentResult
    {
        public FilterAssessmentResult()
        {
            RecordedAnswers = new List<AnswerDetail>();
            SuggestedJobProfiles = new List<string>();
            WhatYouToldUs = new List<string>();
        }

        public string JobFamilyName { get; set; }

        public DateTime CreatedDt { get; set; }

        public string QuestionSetVersion { get; set; }

        public int RecordedAnswerCount { get; set; }

        public int MaxQuestions { get; set; }

        public IEnumerable<AnswerDetail> RecordedAnswers { get; set; }

        public IEnumerable<string> SuggestedJobProfiles { get; set; }

        public IEnumerable<string> WhatYouToldUs { get; set; }
    }
}
