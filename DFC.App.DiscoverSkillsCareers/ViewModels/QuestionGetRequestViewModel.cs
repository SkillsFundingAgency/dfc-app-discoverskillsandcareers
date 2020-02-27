using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class QuestionGetRequestViewModel
    {
        public Assessments AssessmentType { get; set; }

        public int QuestionNumber { get; set; }
    }
}