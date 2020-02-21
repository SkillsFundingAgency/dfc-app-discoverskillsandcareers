using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HomeIndexResponseViewModel
    {
        [Display(Name = "Reference Code")]
        [Required]
        public string ReferenceCode { get; set; }
    }
}
