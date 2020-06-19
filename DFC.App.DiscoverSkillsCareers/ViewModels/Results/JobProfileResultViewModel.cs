using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class JobProfileResultViewModel
    {
        public string JobCategoryName { get; set; }

        public string SocCode { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }
    }
}
