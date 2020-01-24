using System.ComponentModel.DataAnnotations;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class AssessmentSaveRequestViewModel
    {
        [Required]
        public int SaveOption { get; set; }
    }
}
