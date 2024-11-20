using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class ResultData
    {
        public IEnumerable<TraitResult>? Traits { get; set; }

        public IEnumerable<JobCategoryResult>? JobCategories { get; set; }

        public IEnumerable<TraitResult>? LimitedTraits { get; set; }
    }
}
