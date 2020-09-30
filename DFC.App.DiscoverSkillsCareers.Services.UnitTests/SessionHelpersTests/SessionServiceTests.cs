using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Services.SessionHelpers;
using DFC.Compui.Sessionstate;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class SessionServiceTests
    {
        private readonly ISessionStateService<DfcUserSession> fakeStateService;
        private readonly IHttpContextAccessor fakeContextAccessor;
        private readonly SessionService sessionService;

        public SessionServiceTests()
        {
            fakeStateService = A.Fake<ISessionStateService<DfcUserSession>>();
            fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            sessionService = new SessionService(fakeStateService, fakeContextAccessor);
        }

        [Fact]
        public async Task CreateCookie()
        {
            await sessionService.CreateCookie("p1-s1");

            A.CallTo(() => fakeStateService.SaveAsync(A<SessionStateModel<DfcUserSession>>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void GetSessionId()
        {
            var sessionId = Guid.NewGuid();
            A.CallTo(() => fakeContextAccessor.HttpContext.Request.Headers).Returns(new HeaderDictionary(new Dictionary<string, StringValues>(new List<KeyValuePair<string, StringValues>>() { new KeyValuePair<string, StringValues>("x-dfc-composite-sessionid", new StringValues(sessionId.ToString())) })));
            A.CallTo(() => fakeStateService.GetAsync(A<Guid>.Ignored)).Returns(new SessionStateModel<DfcUserSession> { State = new DfcUserSession { SessionId = sessionId.ToString() } });

            var result = await sessionService.GetSessionId().ConfigureAwait(false);

            A.CallTo(() => fakeStateService.GetAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly(); 
            result.Should().Be(sessionId.ToString());
        }

        [Fact]
        public async void GetSessionIdShouldThrowExceptionWhenInvalid()
        {
            SessionStateModel<DfcUserSession>? session = null;

            A.CallTo(() => fakeStateService.GetAsync(A<Guid>.Ignored)).Returns(session);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sessionService.GetSessionId());
        }


        [Theory]
        [InlineData("7d907abb-7eda-4607-824a-529e9ed0a916", true)]
        [InlineData("", false)]
        public async void HasValidSession(string sessionId, bool isValidSession)
        {
            A.CallTo(() => fakeStateService.GetAsync(A<Guid>.Ignored)).Returns(new SessionStateModel<DfcUserSession> { State = new DfcUserSession { SessionId = sessionId } });
            A.CallTo(() => fakeContextAccessor.HttpContext.Request.Headers).Returns(new HeaderDictionary(new Dictionary<string, StringValues>(new List<KeyValuePair<string, StringValues>>() { new KeyValuePair<string, StringValues>("x-dfc-composite-sessionid", new StringValues(sessionId.ToString())) })));

            var result = await sessionService.HasValidSession().ConfigureAwait(false);

            result.Should().Be(isValidSession);

        }
    }
}
