using Dfc.Session;
using Dfc.Session.Models;
using DFC.App.DiscoverSkillsCareers.Services.SessionHelpers;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class SessionServiceTests
    {
        private readonly SessionService sessionService;
        private readonly ISessionClient fakeSessionClient;

        public SessionServiceTests()
        {

            fakeSessionClient = A.Fake<ISessionClient>();
            sessionService = new SessionService(fakeSessionClient);
        }

        [Fact]
        public void CreateCookie()
        {
            sessionService.CreateCookie("p1-s1");

            A.CallTo(() => fakeSessionClient.CreateCookie(A<DfcUserSession>.That.Matches(x => x.PartitionKey == "p1" && x.SessionId == "s1"), false)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void GetSessionId()
        {
            var sessionId = "session1";
            A.CallTo(() => fakeSessionClient.TryFindSessionCode()).Returns(sessionId);
            
            var result = await sessionService.GetSessionId().ConfigureAwait(false);

            result.Should().Be(sessionId);
        }

        [Fact]
        public async void GetSessionIdShouldThrowExceptionWhenInvalid()
        {
            A.CallTo(() => fakeSessionClient.TryFindSessionCode()).Returns("");
            await Assert.ThrowsAsync<InvalidOperationException>(() => sessionService.GetSessionId());
        }


        [Theory]
        [InlineData("s1", true)]
        [InlineData("", false)]
        public async void HasValidSession(string sessionId, bool isValidSession)
        {
            A.CallTo(() => fakeSessionClient.TryFindSessionCode()).Returns(sessionId);

            var result = await sessionService.HasValidSession().ConfigureAwait(false);

            result.Should().Be(isValidSession);

        }
    }
}
