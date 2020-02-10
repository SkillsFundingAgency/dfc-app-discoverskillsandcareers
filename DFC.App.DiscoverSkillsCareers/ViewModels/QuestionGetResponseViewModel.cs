using DFC.App.DiscoverSkillsCareers.Core.Enums;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class QuestionGetResponseViewModel
    {
        public bool IsComplete { get; set; }

        public decimal PercentageComplete { get; set; }

        public string QuestionSetName { get; set; }

        public string QuestionId { get; set; }

        public string QuestionText { get; set; }

        public int? PreviousQuestionNumber { get; set; }

        public string NextQuestionId { get; set; }

        public int? NextQuestionNumber { get; set; }

        public Answer? Answer { get; set; }

        public bool IsChecked(Answer answer)
        {
            return Answer == answer;
        }
    }
}
