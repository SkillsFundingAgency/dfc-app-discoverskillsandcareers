using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.DataProcessorTests
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

        private GetQuestionResponse CreateGetQuestionResponse(int currentQuestionNumber)
        {
            var result = new GetQuestionResponse();
            result.QuestionId = $"short-201901-23-{currentQuestionNumber}";
            result.MaxQuestionsCount = 10;
            return result;
        }
    }
}
