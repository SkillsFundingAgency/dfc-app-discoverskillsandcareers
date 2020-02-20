using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FilterQuestionIndexResponseViewModel
    {
        public QuestionGetResponseViewModel Question { get; set; }

        public string JobCategoryName { get; set; }
    }
}
