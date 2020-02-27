using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DFC.App.DiscoverSkillsCareers.Services
{
    public class CookiePersistanceService : IPersistanceService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CookiePersistanceService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetValue(string key)
        {
            var result = string.Empty;
            if (httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(HeaderNames.Cookie))
            {
                result = httpContextAccessor.HttpContext.Request.Cookies[HeaderNames.Cookie];
            }

            return result;
        }

        public void SetValue(string key, string value)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
        }
    }
}
