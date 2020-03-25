using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultIndexResponseViewModel
    {
        public ResultIndexResponseViewModel()
        {
            Results = new ResultsIndexResponseViewModel();
        }

        public string AssessmentReference { get; set; }

        public ResultsIndexResponseViewModel Results { get; set; }
    }
}
