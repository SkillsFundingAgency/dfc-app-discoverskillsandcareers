using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using DFC.App.DiscoverSkillsCareers.Validation;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class StartViewModel
    {
        [Required]
        public string ReferenceCode { get; set; }

        [Required]
        public string AssessmentStarted { get; set; }

        [ValidateRequired(ErrorMessage = "Choose how to get your reference code sent to you")]
        public AssessmentReturnType? Contact { get; set; }

        [RequiredWhenSelected(Values = new[] { nameof(AssessmentReturnType.Email) }, PropertyName = "Contact", ErrorMessage = "Enter an email address")]
        [ValidateEmailAddress(Values = new[] { nameof(AssessmentReturnType.Email) }, PropertyName = "Contact", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter an email address")]
        public string Email { get; set; }

        [RequiredWhenSelected(Values = new[] { nameof(AssessmentReturnType.Reference) }, PropertyName = "Contact", ErrorMessage = "Enter a UK mobile phone number")]
        [ValidatePhoneNumber(Values = new[] { nameof(AssessmentReturnType.Reference) }, PropertyName = "Contact", ErrorMessage = "Enter a UK mobile phone number, like 07700 900 982.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Enter a UK mobile phone number")]
        public string PhoneNumber { get; set; }

        public DysacAction DysacAction { get; set; }

        public bool IsChecked(AssessmentReturnType answer)
        {
            return Contact == answer;
        }
    }
}
