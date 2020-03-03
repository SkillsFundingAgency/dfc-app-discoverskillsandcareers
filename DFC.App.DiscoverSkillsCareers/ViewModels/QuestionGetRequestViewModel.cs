using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class QuestionGetRequestViewModel
    {
        public string AssessmentType { get; set; }

        public int QuestionNumber { get; set; }
    }
}
