﻿using DFC.App.DiscoverSkillsCareers.Extensions;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Extensions
{
    public class HttpRequestExtensionsTests
    {
        [Fact]
        public void IsRequestFromCompositeReturnsTrueWhenContainsCompositeRequestHeader()
        {
            var request = CreateHttpContext().Request;

            request.Headers.Add("X-Dfc-Composite-Request", "somevalue");

            Assert.True(request.IsRequestFromComposite());
        }

        [Fact]
        public void IsRequestFromCompositeReturnsFalseWhenContainsNoCompositeRequestHeader()
        {
            var request = CreateHttpContext().Request;

            Assert.False(request.IsRequestFromComposite());
        }

        private HttpContext CreateHttpContext()
        {
            var httpContextAccessor = A.Fake<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext = new DefaultHttpContext();
            return httpContext;
        }
    }
}
