using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentSaveRequestViewModel
    {
        [Required]
        [Display(Name = "Return option")]
        public int? ReturnOption { get; set; }
    }
}
