

using DFC.App.DiscoverSkillsCareers.Models.Result;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class ResultJobProfileModel
    {
        public string JobCategory { get; set; }

        public string Title { get; set; }

        public JobProfileOverView JobProfilesOverview { get; set; }
    }
}
