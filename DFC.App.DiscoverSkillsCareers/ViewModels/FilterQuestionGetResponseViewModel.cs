namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class FilterQuestionGetResponseViewModel
    {
        public string JobCategoryName { get; set; }

        public bool IsComplete;

        public decimal PercentageComplete;

        public string QuestionSetName;

        public string QuestionId { get; set; }
        public string QuestionText { get; set; }

        public string PreviousQuestionId { get; set; }
        public string NextQuestionId { get; set; }
    }
}
