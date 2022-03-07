using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultsGetRequestViewModel
    {
        public int CountToShow { get; set; } = 3;
    }
}
