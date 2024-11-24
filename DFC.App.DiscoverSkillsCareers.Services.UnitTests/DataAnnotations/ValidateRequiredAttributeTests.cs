using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.DataAnnotations
{
   public class ValidateRequiredAttributeTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("Email", true)]
        public void CanValidateRequired(string value, bool expectedResult)
        {
            var p = new ValidateRequiredAttribute();
            Assert.Equal(expectedResult, p.IsValid(value));
        }
    }
}
