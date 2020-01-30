namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class FilterQuestionGetResponseViewModel
    {
        public string JobCategoryName { get; set; }

        public bool IsComplete { get; set; }

        public decimal PercentageComplete { get; set; }

        public string QuestionSetName { get; set; }

        public string QuestionId { get; set; }
        public string QuestionText { get; set; }

        public string PreviousQuestionId { get; set; }
        public string NextQuestionId { get; set; }
    }
}
