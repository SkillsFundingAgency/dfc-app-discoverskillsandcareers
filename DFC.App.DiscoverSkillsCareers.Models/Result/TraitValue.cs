using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class TraitValue
    {
        public string TraitCode { get; set; }

        public int Total { get; set; }

        public decimal NormalizedTotal { get; set; }
    }
}
