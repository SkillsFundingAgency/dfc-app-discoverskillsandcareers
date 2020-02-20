using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class BaseDataProcessor
    {
        protected void Process(IQuestion value)
        {
            if (!string.IsNullOrWhiteSpace(value.QuestionId))
            {
                value.QuestionSetName = value.QuestionId.Split("-", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                var lastDashIndex = value.QuestionId.LastIndexOf("-");
                if (lastDashIndex != -1)
                {
                    value.QuestionSetVersion = value.QuestionId.Substring(0, lastDashIndex);

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
