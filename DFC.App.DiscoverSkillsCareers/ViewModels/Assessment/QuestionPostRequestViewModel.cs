using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class QuestionPostRequestViewModel
    {
        public string AssessmentType { get; set; }

        public int QuestionNumber { get; set; }

        [Range(0, 4, ErrorMessage = "Choose an answer to the statement")]
        public int Answer { get; set; } = -1;
    }
}
