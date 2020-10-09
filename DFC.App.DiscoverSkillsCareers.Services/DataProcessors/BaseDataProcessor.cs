using DFC.App.DiscoverSkillsCareers.Models.Assessment;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class BaseDataProcessor
    {
        protected BaseDataProcessor()
        {
        }

        protected static void Process(IQuestion value)
        {
            if (value != null && !string.IsNullOrWhiteSpace(value.QuestionId))
            {
                var lastDashIndex = value.QuestionId.LastIndexOf("-");
                if (lastDashIndex != -1)
                {
                    value.CurrentQuestionNumber = int.Parse(value.QuestionId.Substring(lastDashIndex + 1));

                    value.NextQuestionNumber = value.CurrentQuestionNumber + 1;
                    if (value.NextQuestionNumber >= value.MaxQuestionsCount)
                    {
                        value.NextQuestionNumber = null;
                    }

                    value.PreviousQuestionNumber = value.CurrentQuestionNumber - 1;
                    if (value.PreviousQuestionNumber <= 0)
                    {
                        value.PreviousQuestionNumber = null;
                    }
                }
            }
        }
    }
}
