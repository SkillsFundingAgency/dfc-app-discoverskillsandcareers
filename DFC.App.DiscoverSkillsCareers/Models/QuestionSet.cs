using System.Collections.Generic;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class QuestionSet
    {
        public QuestionSet()
        {
            this.Questions = new List<Question>();
        }

        public string Name { get; set; }

        public List<Question> Questions { get; private set; }

        public int GetQuestionNumber(string id)
        {
            var result = Questions.FindIndex(x => x.Id == id);
            return result == -1 ? 0 : result + 1;
        }

        public int GetCompleted()
        {
            return Questions.Count(x => x.Answer.HasValue);
        }

        public bool IsCompleted()
        {
            return Questions.All(x => x.Answer.HasValue);
        }

        public Question GetQuestion(string id)
        {
            return Questions.FirstOrDefault(x => x.Id == id);
        }

        public Question GetPreviousQuestion(string id)
        {
            Question result = null;
            var currentQuestionIndex = Questions.FindIndex(x => x.Id == id);
            if (currentQuestionIndex != -1)
            {
                result = Questions.ElementAtOrDefault(currentQuestionIndex - 1);
            }

            return result;
        }

        public Question GetNextQuestion(string id)
        {
            Question result = null;
            var currentQuestionIndex = Questions.FindIndex(x => x.Id == id);
            if (currentQuestionIndex != -1)
            {
                result = Questions.ElementAtOrDefault(currentQuestionIndex + 1);
            }

            return result;
        }
    }
}
