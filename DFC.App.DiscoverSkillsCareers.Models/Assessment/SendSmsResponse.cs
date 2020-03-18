using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.Models.Assessment
{
    [ExcludeFromCodeCoverage]
    public class SendSmsResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
