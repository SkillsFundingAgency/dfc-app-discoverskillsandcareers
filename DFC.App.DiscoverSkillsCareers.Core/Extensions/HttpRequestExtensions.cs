using DFC.App.DiscoverSkillsCareers.Core.Constants;
using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsRequestFromComposite(this HttpRequest httpRequest)
        {
            return httpRequest.Headers.ContainsKey(HeaderName.CompositeRequest);
        }
    }
}
