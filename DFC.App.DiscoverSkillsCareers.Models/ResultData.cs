using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class ResultData
    {
        public IEnumerable<TraitResult>? Traits { get; set; }

        public IEnumerable<JobCategoryResult>? JobCategories { get; set; }

        public IEnumerable<TraitResult>? TraitScores { get; set; }

        public IEnumerable<JobProfileResult>? JobProfiles { get; set; }
        public IEnumerable<string?> TraitText { get; set; }
    }
}
