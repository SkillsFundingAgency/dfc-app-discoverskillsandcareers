using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ResultJobProfileOverViewModel
    {
        public string Cname { get; set; }

        public string OverViewHTML { get; set; }

        public HttpStatusCode ReturnedStatusCode { get; set; }
    }
}
