using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AssessmentReferenceGetResponse
    {
        public string ReferenceCode { get; set; }

        public string AssessmentStarted { get; set; }

        [Required(ErrorMessage = "Enter a phone number")]
        [RegularExpression(@"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$", 
            ErrorMessage = "Enter a mobile phone number, like 07700 900 982.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Enter a phone number")]
        public string Telephone { get; set; }
    }
}
