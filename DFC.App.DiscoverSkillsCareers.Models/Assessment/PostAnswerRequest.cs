using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class PostAnswerRequest
    {
        public string QuestionId { get; set; }

        public string SelectedOption { get; set; }
    }
}
