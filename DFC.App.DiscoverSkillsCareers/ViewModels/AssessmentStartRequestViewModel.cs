using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentStartRequestViewModel
    {
        [Required]
        public string QuestionSetName { get; set; }

        [Required]
        public string QuestionId { get; set; }

        public AssessmentStartRequestViewModel()
        {
            QuestionSetName = "short";
            QuestionId = "01";
        }
    }
}
