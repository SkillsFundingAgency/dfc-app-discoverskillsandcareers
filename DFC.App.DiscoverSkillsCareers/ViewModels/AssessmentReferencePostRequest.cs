using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentReferencePostRequest
    {
        [Required]
        public string Telephone { get; set; }
    }
}
