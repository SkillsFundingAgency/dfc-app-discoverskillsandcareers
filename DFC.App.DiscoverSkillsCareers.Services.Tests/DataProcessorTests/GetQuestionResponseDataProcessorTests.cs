using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.Tests.AssessmentApiServiceTests
{
    public class GetQuestionResponseDataProcessorTests
    {
        [Theory]
        [InlineData(1, null, 2)]
        [InlineData(2, 1, 3)]
        [InlineData(10, 9, null)]
        public void CanSetPreviousAndNextPageCounts(int currentQuestionNumber, int? expectedPreviousQuestionNumber, int? expectedNextQuestionNumber)
        {
            var data = CreateGetQuestionResponse(currentQuestionNumber);
            var dataProcessor = new GetQuestionResponseDataProcessor();
            dataProcessor.Processor(data);

            Assert.Equal(expectedPreviousQuestionNumber, data.PreviousQuestionNumber);
            Assert.Equal(expectedNextQuestionNumber, data.NextQuestionNumber);
        }

        [Fact]
        public void CanGetQuestionSetDetails()
        {
            var data = CreateGetQuestionResponse(1);
            var dataProcessor = new GetQuestionResponseDataProcessor();
            dataProcessor.Processor(data);

            Assert.Equal("short", data.QuestionSetName);
            Assert.Equal("short-201901-23", data.QuestionSetVersion);
        }

        private GetQuestionResponse CreateGetQuestionResponse(int currentQuestionNumber)
        {
            var result = new GetQuestionResponse();
            result.QuestionId = $"short-201901-23-{currentQuestionNumber}";
            result.MaxQuestionsCount = 10;
            return result;
        }
    }
}
