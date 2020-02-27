using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Services.Persistance
{
    public class CookiePersistantService : IPersistanceService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CookiePersistantService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetValue(string key)
        {
            var result = string.Empty;
            if (httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(key))
            {
                result = httpContextAccessor.HttpContext.Request.Cookies[key];
            }

            return result;
        }

        public void SetValue(string key, string value)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
        }
    }
}
