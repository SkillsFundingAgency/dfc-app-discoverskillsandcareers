using DFC.App.DiscoverSkillsCareers.Services.Helpers;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.SessionHelperTests
{
    public class SessionHelperTests
    {
        [Fact]
        public void SessionHelperFormatSessionIdReturnsCorrectFormat()
        {
            // Arrange
            var unformattedSessionId = "abcd1234abcd12";

            // Act
            var result = SessionHelper.FormatSessionId(unformattedSessionId);

            // Assert
            Assert.Equal("ABCD 1234 ABCD 12", result);
        }
    }
}