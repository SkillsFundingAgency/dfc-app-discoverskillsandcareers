using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class TraitValueViewModel
    {
        public string TraitCode { get; set; }

        public int Total { get; set; }

        public decimal NormalizedTotal { get; set; }
    }
}
