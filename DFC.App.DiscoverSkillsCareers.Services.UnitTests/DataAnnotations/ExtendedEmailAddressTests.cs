using DFC.App.DiscoverSkillsCareers.Services.DataAnnotations;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.DataAnnotations
{
    public class ExtendedEmailAddressTests
    {
        [Theory]
        [InlineData("abc@def.com", true)]
        [InlineData("abc123@def.com", true)]
        [InlineData("abc.123@def.com", true)]
        [InlineData("abc@def123.com", true)]
        [InlineData("abc123@def123.com", true)]

        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("a", false)]
        [InlineData("456", false)]
        [InlineData("@", false)]
        [InlineData("@defcom", false)]
        [InlineData("abc@defcom", false)]
        [InlineData("abc@def@com", false)]
        public void CanValidate(string value, bool expectedResult)
        {
            var p = new ExtendedEmailAddressAttribute();
            Assert.Equal(expectedResult, p.IsValid(value));
        }
    }
}
