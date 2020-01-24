using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class QuestionPostRequestViewModel
    {
        public string QuestionSetName { get; set; }

        public string QuestionId { get; set; }

        [Required(ErrorMessage = "Choose an answer to the statement")]
        public int? Answer { get; set; }
    }
}
