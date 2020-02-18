using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentSaveRequestViewModel
    {
        [Display(Name = "Return option")]
        [Required]
        public AssessmentReturnType? AssessmentReturnTypeId { get; set; }
    }
}
