using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class HomeIndexRequestViewModel
    {
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }
    }
}
