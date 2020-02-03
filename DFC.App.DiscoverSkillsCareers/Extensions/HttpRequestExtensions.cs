using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsRequestFromComposite(this HttpRequest httpRequest)
        {
            var headerName = "X-Dfc-Composite-Request";
            return httpRequest.Headers.ContainsKey(headerName);
        }
    }
}
