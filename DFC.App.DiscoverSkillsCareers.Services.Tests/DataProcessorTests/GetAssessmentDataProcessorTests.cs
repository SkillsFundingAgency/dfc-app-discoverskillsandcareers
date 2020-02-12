using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.Tests.AssessmentApiServiceTests
{
    public class GetAssessmentDataProcessorTests
    {
        [Theory]
        [InlineData(1, null, 2)]
        [InlineData(2, 1, 3)]
        [InlineData(10, 9, null)]
        public void CanSetPreviousAndNextPageCounts(int currentQuestionNumber, int? expectedPreviousQuestionNumber, int? expectedNextQuestionNumber)
        {
            var response = CreateAssessmentResponse(currentQuestionNumber);
            var dataProcessor = new GetAssessmentResponseDataProcessor();
            dataProcessor.Processor(response);

            Assert.Equal(expectedPreviousQuestionNumber, response.PreviousQuestionNumber);
            Assert.Equal(expectedNextQuestionNumber, response.NextQuestionNumber);
        }

        [Fact]
        public void CanGetDetails()
        {
            var data = CreateAssessmentResponse(1);
            var dataProcessor = new GetAssessmentResponseDataProcessor();
            dataProcessor.Processor(data);

            Assert.Equal("short", data.QuestionSetName);
            Assert.Equal("short-201901-23", data.QuestionSetVersion);
        }

        private GetAssessmentResponse CreateAssessmentResponse(int currentQuestionNumber)
        {
            var result = new GetAssessmentResponse();
            result.QuestionId = $"short-201901-23-{currentQuestionNumber}";
            result.MaxQuestionsCount = 10;
            return result;
        }
    }
}
