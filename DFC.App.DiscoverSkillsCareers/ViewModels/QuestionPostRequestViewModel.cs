using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class QuestionPostRequestViewModel
    {
        public string QuestionSetName { get; set; }

        public int QuestionNumber { get; set; }

        [Required(ErrorMessage = "Choose an answer to the statement")]
        public string Answer { get; set; }
    }
}
