using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class FilterQuestionPostRequestViewModel
    {
        public string AssessmentType { get; set; }

        public int QuestionNumberReal { get; set; }

        public int QuestionNumberCounter { get; set; }

        [Required(ErrorMessage = "Choose an answer to the statement")]
        public string Answer { get; set; }

        public string JobCategoryName { get; set; }
    }
}
