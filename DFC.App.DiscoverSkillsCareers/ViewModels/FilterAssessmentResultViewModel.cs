using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FilterAssessmentResultViewModel
    {
        public FilterAssessmentResultViewModel()
        {
            RecordedAnswers = new List<AnswerDetailsViewModel>();
            SuggestedJobProfiles = new List<string>();
            WhatYouToldUs = new List<string>();
        }

        public string JobFamilyName { get; set; }

        public DateTime CreatedDt { get; set; }

        public string QuestionSetVersion { get; set; }

        public int RecordedAnswerCount { get; set; }

        public int MaxQuestions { get; set; }

        public IEnumerable<AnswerDetailsViewModel> RecordedAnswers { get; set; }

        public IEnumerable<string> SuggestedJobProfiles { get; set; }

        public IEnumerable<string> WhatYouToldUs { get; set; }
    }
}
