using DFC.App.DiscoverSkillsCareers.Core.Constants;
using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsRequestFromComposite(this HttpRequest httpRequest)
        {
            var result = false;
            if (httpRequest != null && httpRequest.Headers != null)
            {
                result = httpRequest.Headers.ContainsKey(HeaderName.CompositeRequest);
            }

            return result;
        }
    }
}
