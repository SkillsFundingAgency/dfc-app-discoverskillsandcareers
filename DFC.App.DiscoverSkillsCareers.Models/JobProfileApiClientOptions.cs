using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class JobProfileApiClientOptions : ClientOptionsModel
    {
        public string SummaryEndpoint { get; set; } = "/page";

        public string StaticContentEndpoint { get; set; } = "/sharedcontent/";

        public string? ContentIds { get; set; }
    }
}
