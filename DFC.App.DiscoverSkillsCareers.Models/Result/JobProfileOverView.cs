using System.Net;

namespace DFC.App.DiscoverSkillsCareers.Models.Result
{
    public class JobProfileOverView
    {
        public string? Cname { get; set; }

        public string? OverViewHTML { get; set; }

        public HttpStatusCode ReturnedStatusCode { get; set; }
    }
}
