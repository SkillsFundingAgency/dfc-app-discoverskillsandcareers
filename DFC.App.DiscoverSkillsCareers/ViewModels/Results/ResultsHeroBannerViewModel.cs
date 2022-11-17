using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultsHeroBannerViewModel
    {
        public IEnumerable<string> Traits { get; set; }

        public bool IsCategoryBanner { get; set; } = true;

        public int NumberOfCategories { get; set; }
    }
}
