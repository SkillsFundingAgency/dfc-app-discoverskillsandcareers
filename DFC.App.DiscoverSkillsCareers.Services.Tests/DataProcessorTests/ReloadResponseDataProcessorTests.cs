using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.Tests.AssessmentApiServiceTests
{
    public class ReloadResponseDataProcessorTests
    {
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
        public void CanFormatCode(string reloadCode, string expectedReferenceCode)
        {
            var data = CreateGetAssessmentResponse(reloadCode);
            var dataProcessor = new GetAssessmentResponseDataProcessor();
            dataProcessor.Processor(data);

            Assert.Equal(expectedReferenceCode, data.ReferenceCode);
        }

        private GetAssessmentResponse CreateGetAssessmentResponse(string reloadCode)
        {
            var result = new GetAssessmentResponse();
            result.ReloadCode = reloadCode;
            return result;
        }
    }
}
