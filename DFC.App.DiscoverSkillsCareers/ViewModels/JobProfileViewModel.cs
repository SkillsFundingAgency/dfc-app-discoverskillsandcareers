using System;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class JobProfileViewModel
    {
        public string DisplayText { get; set; }

        public string Overview { get; set; }

        public string Html { get; set; }

        public int SalaryStarterPerYear { get; set; }

        public int SalaryExperiencedPerYear { get; set; }

        public string UrlName { get; set; }

        public Double MaximumHours { get; set; }

        public Double MinimumHours { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPatternDetails { get; set; }
    }
}
