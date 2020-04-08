using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AssessmentEmailPostRequest
    {
        [Required]
        [ExtendedEmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
