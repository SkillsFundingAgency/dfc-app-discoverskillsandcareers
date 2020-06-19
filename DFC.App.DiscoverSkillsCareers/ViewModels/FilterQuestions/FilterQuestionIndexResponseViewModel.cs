using DFC.App.DiscoverSkillsCareers.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FilterQuestionIndexResponseViewModel
    {
        public FilterQuestionIndexResponseViewModel()
        {
            Question = new QuestionGetResponseViewModel();
        }

        public QuestionGetResponseViewModel Question { get; set; }

        [Required(ErrorMessage = "Choose an answer to the statement")]
        public Answer? Answer { get; set; }

        public string JobCategoryName { get; set; }
    }
}
