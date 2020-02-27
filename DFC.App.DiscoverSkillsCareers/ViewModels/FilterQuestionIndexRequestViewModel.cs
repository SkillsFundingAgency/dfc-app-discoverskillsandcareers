using DFC.App.DiscoverSkillsCareers.Core;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FilterQuestionIndexRequestViewModel
    {
        public string JobCategoryName { get; set; }

        public int QuestionNumber { get; set; }
    }
}