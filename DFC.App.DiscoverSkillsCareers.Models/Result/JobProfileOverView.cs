using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    [ExcludeFromCodeCoverage]
    public class JobProfileOverView
    {
        public string? Cname { get; set; }

        public string? OverViewHTML { get; set; }

        public HttpStatusCode ReturnedStatusCode { get; set; }
    }
}
