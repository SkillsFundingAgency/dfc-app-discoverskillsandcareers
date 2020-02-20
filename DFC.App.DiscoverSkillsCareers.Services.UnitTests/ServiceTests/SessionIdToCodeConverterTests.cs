using DFC.App.DiscoverSkillsCareers.Services.SessionIdToCodeConverters;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class SessionIdToCodeConverterTests
    {
        private readonly SessionIdToCodeConverter defaultSessionIdToCodeConverter;

        public SessionIdToCodeConverterTests()
        {
            defaultSessionIdToCodeConverter = new SessionIdToCodeConverter();
        }

        [Theory]
        [InlineData("e", "E")]
        [InlineData("ez", "EZ")]
        [InlineData("ezw", "EZW")]
        [InlineData("ezw6", "EZW6")]
        [InlineData("ezw6 ", "EZW6")]
        [InlineData("ezw68", "EZW6 8")]
        [InlineData("ezw689", "EZW6 89")]
        [InlineData("ezw689 ", "EZW6 89")]
        [InlineData("ezw68mmyw943m3", "EZW6 8MMY W943 M3")]
        public void CanGetCode(string source, string expectedCode)
        {
            var actualCode=defaultSessionIdToCodeConverter.GetCode(source);
            Assert.Equal(expectedCode, actualCode);
        }

        [Theory]
        [InlineData("EZW6 8MMY W943 M3", "ezw68mmyw943m3")]
        [InlineData("EZW6 8MMY W943 m3", "ezw68mmyw943m3")]
        [InlineData(" EZW6 8MMY W943 m3 ", "ezw68mmyw943m3")]
        public void CanGetSessionId(string source, string expectedSessionId)
        {
            var actualSessionId = defaultSessionIdToCodeConverter.GetSessionId(source);
            Assert.Equal(expectedSessionId, actualSessionId);
        }
    }
}
