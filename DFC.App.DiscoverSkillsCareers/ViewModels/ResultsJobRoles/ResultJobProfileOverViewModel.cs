using System.Net;

namespace DFC.App.DiscoverSkillsCareers.ViewModels
{
    public class ResultJobProfileOverViewModel
    {
        public string Cname { get; set; }

        public string OverViewHTML { get; set; }

        public HttpStatusCode ReturnedStatusCode { get; set; }
    }
}
