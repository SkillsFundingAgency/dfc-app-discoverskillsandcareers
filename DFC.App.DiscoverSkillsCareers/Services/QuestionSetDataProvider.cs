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

            var questionSetShort = new QuestionSet() { Name = "short" };
            questionSetShort.Questions.Add(new Question() { Id = "01", Text = "I am comfortable telling people what they need to do" });
            questionSetShort.Questions.Add(new Question() { Id = "02", Text = "I make decisions quickly" });
            questionSetShort.Questions.Add(new Question() { Id = "03", Text = "I like to take control of situations" });
            questionSets.Add(questionSetShort);

            var questionSetSportsAndLeisure = new QuestionSet() { Name = "sports-and-leisure" };
            questionSetSportsAndLeisure.Questions.Add(new Question() { Id = "01", Text = "Are you comfortable talking through things with other people so that they understand?" });
            questionSetSportsAndLeisure.Questions.Add(new Question() { Id = "02", Text = "Are you able to control your emotions even in difficult situations?" });
            questionSetSportsAndLeisure.Questions.Add(new Question() { Id = "03", Text = "Do you think you are good at staying calm under pressure?" });
            questionSets.Add(questionSetSportsAndLeisure);

            var questionSetManufacturing = new QuestionSet() { Name = "manufacturing" };
            questionSetManufacturing.Questions.Add(new Question() { Id = "01", Text = "Are you comfortable working in a team with other people?" });
            questionSetManufacturing.Questions.Add(new Question() { Id = "02", Text = "Do you think you are good at using words to describe ideas?" });
            questionSetManufacturing.Questions.Add(new Question() { Id = "03", Text = "Are you able to control your emotions even in difficult situations?" });
            questionSetManufacturing.Questions.Add(new Question() { Id = "04", Text = "Are you able to do detailed, intricate work with your hands?" });
            questionSets.Add(questionSetManufacturing);
        }

        public QuestionSet GetQuestionSet(string name)
        {
            return questionSets.FirstOrDefault(x => x.Name == name);
        }
    }
}
