using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class JobProfileResultViewModel
    {
        public string JobCategory { get; set; }

        public string SocCode { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public decimal SalaryStarter { get; set; }

        public decimal SalaryExperienced { get; set; }

        public string UrlName { get; set; }

        public string WYDDayToDayTasks { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string TypicalHours { get; set; }

        public string TypicalHoursPeriod { get; set; }

        public string ShiftPattern { get; set; }

        public string ShiftPatternPeriod { get; set; }

        public bool IsVariable { get; set; }
    }
}
