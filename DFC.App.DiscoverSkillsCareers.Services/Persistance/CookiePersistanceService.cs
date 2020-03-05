using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.AspNetCore.Http;

namespace DFC.App.DiscoverSkillsCareers.Services.Persistance
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
            return httpContextAccessor.HttpContext.Request.Cookies[key];
        }

        public void SetValue(string key, string value)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
        }
    }
}
