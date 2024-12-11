using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryCardViewModel
    {
        public JobCategoryResultViewModel JobCategory { get; set; }

        public string AssessmentType { get; set; }
    }
}