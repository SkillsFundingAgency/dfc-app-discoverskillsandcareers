using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class TraitResult
    {
        public string? TraitCode { get; set; }

        public int TotalScore { get; set; }

        public string? Text { get; set; }

        public string? ImagePath { get; set; }
    }
}
