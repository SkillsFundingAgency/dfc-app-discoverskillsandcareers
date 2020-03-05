using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Linq;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.PersistanceTests
{
    public class CookiePersistanceServiceTests
    {
        private readonly IPersistanceService cookiePersistanceService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CookiePersistanceServiceTests()
        {
            httpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = new DefaultHttpContext()
            };
            cookiePersistanceService = new CookiePersistanceService(httpContextAccessor);
        }

        [Fact]
        public void CanSetCookie()
        {
            var key = "key1";
            var value = "value1";

            cookiePersistanceService.SetValue(key, value);

            var headers = httpContextAccessor.HttpContext.Response.Headers[HeaderNames.SetCookie];
            var containsValue = headers.First().Contains($"{key}={value}");
            Assert.True(containsValue);
        }

        [Fact]
        public void CanGetCookie()
        {
            var key = "key1";
            var value = "value1";

            httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Cookie] = $"{key}={value}";
            var peristedValue = cookiePersistanceService.GetValue(key);

            Assert.Equal(value, peristedValue);
        }
    }
}
