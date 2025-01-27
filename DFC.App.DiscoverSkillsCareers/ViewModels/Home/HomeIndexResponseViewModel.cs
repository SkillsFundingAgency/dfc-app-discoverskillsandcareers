using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using DFC.App.DiscoverSkillsCareers.Validation;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class HomeIndexResponseViewModel
    {
        public string SpeakToAnAdviser { get; set; }

        [Display(Name = "Reference Code")]
        [ValidateRequired(ErrorMessage = "Enter your reference code")]
        [ReferenceCodeExists(ErrorMessage = "Enter a valid reference code")]
        public string ReferenceCode { get; set; }
    }
}
