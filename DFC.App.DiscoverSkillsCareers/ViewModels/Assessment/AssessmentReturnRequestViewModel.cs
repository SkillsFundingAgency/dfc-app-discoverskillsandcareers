using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AssessmentReturnRequestViewModel
    {
        [Required]
        public int Reference { get; set; }
    }
}
