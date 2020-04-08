using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FilterBodyTopViewModel
    {
        public string JobCategoryName { get; set; }

        public int QuestionNumber { get; set; }

        public string AssessmentType { get; set; }
    }
}
