using DFC.App.DiscoverSkillsCareers.Core.Enums;
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

        [Required(ErrorMessage = "Choose how to get your reference code sent to you")]
        public AssessmentReturnType? Contact { get; set; }

        [RequiredWhenSelectedAttribute(Values = new[] { nameof(AssessmentReturnType.Email) }, PropertyName = "Contact", ErrorMessage = "Enter a email")]
        [RegularExpression(
            @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$",
            ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter a email")]
        public string Email { get; set; }

        [RequiredWhenSelectedAttribute(Values = new[] { nameof(AssessmentReturnType.Reference) }, PropertyName = "Contact", ErrorMessage = "Enter a phone number")]
        [RegularExpression(
            @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$",
            ErrorMessage = "Enter a mobile phone number, like 07700 900 982.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Enter a phone number")]
        public string PhoneNumber { get; set; }

        public bool IsChecked(AssessmentReturnType answer)
        {
            return Contact == answer;
        }
    }
}
