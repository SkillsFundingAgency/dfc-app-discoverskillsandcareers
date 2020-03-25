using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AssessmentSaveRequestViewModel
    {
        [Display(Name = "Return option")]
        [Required]
        public AssessmentReturnType? AssessmentReturnTypeId { get; set; }
    }
}
