using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AssessmentSaveResponseViewModel
    {
        [Display(Name = "Return option")]
        [Required(ErrorMessage = "Choose how you would like to return to your assessment")]
        public AssessmentReturnType? AssessmentReturnTypeId { get; set; }
    }
}
