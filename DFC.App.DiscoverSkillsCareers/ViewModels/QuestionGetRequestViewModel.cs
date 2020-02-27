using DFC.App.DiscoverSkillsCareers.Core;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class QuestionGetRequestViewModel
    {
        public AssessmentType AssessmentType { get; set; }

        public int QuestionNumber { get; set; }
    }
}
