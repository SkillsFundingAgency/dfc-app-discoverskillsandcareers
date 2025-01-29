using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.DataAnnotations
{
   public class ValidateEmailAddressAttributeTests
    {
        [Theory]
        [InlineData("abc@gmail.com", new[] { nameof(AssessmentReturnType.Email) })]
        public void CanValidate(object value,string[] allowedValues)
        {           
            var obj  = new StartViewModel() { Contact = AssessmentReturnType.Email };
            
            var validationContext = new ValidationContext(obj)
            {
                MemberName = nameof(AssessmentReturnType.Email),
            };

            var p = new ValidateEmailAddressAttribute
            {
                ErrorMessage = "Enter an email address in the correct format, like name@example.com",
                PropertyName = "Contact",
                Values = allowedValues
               
            };
            var result = p.GetValidationResult(value, validationContext);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("abc@g", new[] { nameof(AssessmentReturnType.Email) })]
        public void CanValidateMail(object value, string[] allowedValues)
        {
            var obj = new StartViewModel() { Contact = AssessmentReturnType.Email };

            var validationContext = new ValidationContext(obj)
            {
                MemberName = nameof(AssessmentReturnType.Email),
            };

            var p = new ValidateEmailAddressAttribute
            {
                ErrorMessage = "Enter an email address in the correct format, like name@example.com",
                PropertyName = "Contact",
                Values = allowedValues

            };
            var result = p.GetValidationResult(value, validationContext);
            Assert.NotNull(result);
            Assert.Equal(result?.ErrorMessage, p.ErrorMessage);
        }
    }
}
