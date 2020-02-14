using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AnswerDetailsViewModel
    {
        public string QuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string QuestionText { get; set; }

        public string TraitCode { get; set; }

        public Answer SelectedOption { get; set; }

        public DateTime AnsweredDt { get; set; }

        public bool IsNegative { get; set; }

        public string QuestionSetVersion { get; set; }
    }
}
