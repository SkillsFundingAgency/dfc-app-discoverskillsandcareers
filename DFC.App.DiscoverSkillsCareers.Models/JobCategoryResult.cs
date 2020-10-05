using NHibernate.Mapping;
using System.Collections.Generic;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class JobCategoryResult
    {
        public string? JobCategoryName { get; set; }

        public string? JobCategoryText { get; set; }

        public string? Url { get; set; }

        public int? TraitsTotal { get; set; }

        public int? TotalQuestions { get; set; }

        public int? Total { get; set; }

        public IEnumerable<string>? TraitValues { get; set; }

        public int? NormalizedTotal { get; set; }
    }
}
