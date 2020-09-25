using DFC.App.DiscoverSkillsCareers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services.Helpers
{
    public static class QuestionHelpers
    {
        public static int? GetNextQuestionNumber(IEnumerable<ShortQuestion> questions)
        {
            var nextQuestion = questions.Where(x => !x.IsComplete).OrderBy(x => x.Ordinal).FirstOrDefault();

            if (nextQuestion != null)
            {
                //Is this the question after this one?
                return nextQuestion.Ordinal + 1;
            }

            return null;
        }

        internal static ShortQuestion? GetNextQuestion(IEnumerable<ShortQuestion> questions)
        {
            var nextQuestion = questions.Where(x => !x.IsComplete).OrderBy(x => x.Ordinal).FirstOrDefault();

            if (nextQuestion != null)
            {
                return nextQuestion;
            }

            return null;
        }

        internal static int GetPercentComplete(IEnumerable<ShortQuestion> questions)
        {
            throw new NotImplementedException();
        }
    }
}
