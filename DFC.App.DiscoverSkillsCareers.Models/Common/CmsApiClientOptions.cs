namespace DFC.App.DiscoverSkillsCareers.Models.Common
{
    public class CmsApiClientOptions : ClientOptionsModel
    {
        public string SummaryEndpoint { get; set; } = "content/getcontent/api/execute/html";
    }
}
