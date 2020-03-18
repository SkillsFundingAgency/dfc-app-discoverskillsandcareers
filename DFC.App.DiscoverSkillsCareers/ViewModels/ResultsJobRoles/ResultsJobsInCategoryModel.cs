using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class ResultsJobsInCategoryModel
    {
        public string CategoryUrl { get; set; }

        public string CategoryTitle { get; set; }

        public int NumberOfSuitableRoles { get; set; }

        public int DisplayOrder { get; set; }

        public bool ShowThisCategory { get; set; }

        public int AnswerMoreQuetions { get; set; }

        public IEnumerable<ResultJobProfileOverViewModel> JobProfiles { get; set; }
    }
}
