using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using System;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.HelperTests
{
    public class SessionIdHelperTests
    {
        [Fact]
        public void SessionIdHelperGenerateSessionIdFromSaltReturnsSessionId()
        {
            // Arrange
            // Act
            var result = SessionIdHelper.GenerateSessionId("testsalt");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(14, result.Length);
        }

        [Fact]
        public void SessionIdHelperGenerateSessionIdFromSaltAndDateReturnsSessionId()
        {
            // Arrange
            // Act
            var result = SessionIdHelper.GenerateSessionId("testsalt", DateTime.UtcNow);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(14, result.Length);
        }

        [Fact]
        public void SessionIdHelperDecodeReturnsDecodedSessionId()
        {
            // Arrange
            // Act
            var sessionId = SessionIdHelper.GenerateSessionId("testsalt", DateTime.UtcNow);
            string decodedSessionId = SessionIdHelper.Decode("testsalt", sessionId);

            // Assert
            Assert.NotNull(decodedSessionId);
            Assert.Equal(16, decodedSessionId.Length);
        }
    }
}
