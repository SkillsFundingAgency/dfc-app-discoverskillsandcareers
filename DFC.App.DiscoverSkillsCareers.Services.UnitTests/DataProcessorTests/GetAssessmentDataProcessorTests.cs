using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using FakeItEasy;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.DataProcessorTests
{
    public class GetAssessmentDataProcessorTests
    {
        private readonly GetAssessmentResponseDataProcessor dataProcessor;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;

        public GetAssessmentDataProcessorTests()
        {
            sessionIdToCodeConverter = A.Fake<ISessionIdToCodeConverter>();
            dataProcessor = new GetAssessmentResponseDataProcessor(sessionIdToCodeConverter);
        }

        [Theory]
        [InlineData(1, null, 2)]
        [InlineData(2, 1, 3)]
        [InlineData(10, 9, null)]
        public void CanSetPreviousAndNextPageCounts(int currentQuestionNumber, int? expectedPreviousQuestionNumber, int? expectedNextQuestionNumber)
        {
            var response = CreateAssessmentResponse(currentQuestionNumber);
            dataProcessor.Processor(response);

            Assert.Equal(expectedPreviousQuestionNumber, response.PreviousQuestionNumber);
            Assert.Equal(expectedNextQuestionNumber, response.NextQuestionNumber);
        }

        [Fact]
        public void CanGetDetails()
        {
            var data = CreateAssessmentResponse(1);
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
