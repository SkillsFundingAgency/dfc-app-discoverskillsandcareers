using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentReturnRequestViewModel
    {
        [Required]
        public int Reference { get; set; }
    }
}
