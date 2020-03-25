using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class FilterAssessmentResponse
    {
        public string SessionId { get; set; }

        public int QuestionNumber { get; set; }
    }
}
