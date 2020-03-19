using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultJobProfileModel
    {
        public string JobCategory { get; set; }

        public string Title { get; set; }

        public JobProfileOverView JobProfilesOverview { get; set; }
    }
}
