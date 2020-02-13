using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class HomeIndexRequestViewModel
    {
        [Display(Name = "Reference Code")]
        [Required]
        public string ReferenceCode { get; set; }
    }
}
