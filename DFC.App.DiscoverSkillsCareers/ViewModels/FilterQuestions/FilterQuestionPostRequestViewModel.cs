using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FilterQuestionPostRequestViewModel
    {
        public string AssessmentType { get; set; }

        public int QuestionNumberReal { get; set; }

        public int QuestionNumberCounter { get; set; }

        [Required(ErrorMessage = "Choose an answer to the statement")]
        public int Answer { get; set; }

        public string JobCategoryName { get; set; }
    }
}
