using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentEmailPostRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
