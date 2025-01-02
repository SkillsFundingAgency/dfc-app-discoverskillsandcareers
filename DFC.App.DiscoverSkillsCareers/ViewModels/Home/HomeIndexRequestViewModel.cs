using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HomeIndexRequestViewModel
    {
        [Display(Name = "Reference Code")]
        [ValidateRequired(ErrorMessage = "Enter your reference")]
        public string ReferenceCode { get; set; }
    }
}
