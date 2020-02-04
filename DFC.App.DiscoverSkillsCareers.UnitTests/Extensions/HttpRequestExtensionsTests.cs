using DFC.App.DiscoverSkillsCareers.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Extensions
{
    public class HttpRequestExtensionsTests
    {
        [Fact]
        public void IsRequestFromCompositeReturnsTrueWhenContainsCompositeRequestHeader()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.Object.HttpContext = new DefaultHttpContext();

            var request = httpContext.Request;
            request.Headers.Add("X-Dfc-Composite-Request", "somevalue");

            Assert.True(request.IsRequestFromComposite());
        }

        [Fact]
        public void IsRequestFromCompositeReturnsFalseWhenContainsNoCompositeRequestHeader()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.Object.HttpContext = new DefaultHttpContext();

            var request = httpContext.Request;

            Assert.False(request.IsRequestFromComposite());
        }
    }
}
