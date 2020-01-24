using DFC.App.DiscoverSkillsCareers.Models;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public class QuestionSetDataProvider
    {
        private List<QuestionSet> questionSets;

        public QuestionSetDataProvider()
        {
            questionSets = new List<QuestionSet>();

            var questionSet1 = new QuestionSet() { Name = "short" };
            questionSet1.Questions.Add(new Question() { Id = "01", Text = "I am comfortable telling people what they need to do" });
            questionSet1.Questions.Add(new Question() { Id = "02", Text = "I make decisions quickly" });
            questionSet1.Questions.Add(new Question() { Id = "03", Text = "I like to take control of situations" });

            questionSets.Add(questionSet1);
        }

        public QuestionSet GetQuestionSet(string name)
        {
            return questionSets.FirstOrDefault(x => x.Name == name);
        }
    }
}
