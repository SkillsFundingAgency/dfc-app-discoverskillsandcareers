using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.DataAnnotations
{
    public class ValidatePhoneNumberAttributeTests
    {
        [Theory]
        [InlineData("", new[] { nameof(AssessmentReturnType.Reference) })]
        public void CanValidate(object value, string[] allowedValues)
        {
            var obj = new StartViewModel() { Contact = AssessmentReturnType.Reference };

            var validationContext = new ValidationContext(obj)
            {
                MemberName = nameof(AssessmentReturnType.Reference),
            };

            var p = new ValidatePhoneNumberAttribute
            {
                ErrorMessage = "Enter a UK mobile phone number, like 07700 900 982.",
                PropertyName = "Contact",
                Values = allowedValues

            };
            var result = p.GetValidationResult(value, validationContext);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("1234", new[] { nameof(AssessmentReturnType.Reference) })]
        public void CanValidatePhoneNumber(object value, string[] allowedValues)
        {
            var obj = new StartViewModel() { Contact = AssessmentReturnType.Reference };

            var validationContext = new ValidationContext(obj)
            {
                MemberName = nameof(AssessmentReturnType.Reference),
            };

            var p = new ValidatePhoneNumberAttribute
            {
                ErrorMessage = "Enter a UK mobile phone number, like 07700 900 982.",
                PropertyName = "Contact",
                Values = allowedValues

            };
            var result = p.GetValidationResult(value, validationContext);
            Assert.NotNull(result);
            Assert.Equal(result?.ErrorMessage, p.ErrorMessage);
        }
    }
}
