using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class PostAnswerRequest
    {
        public string? QuestionId { get; set; }

        public int SelectedOption { get; set; }
    }
}
