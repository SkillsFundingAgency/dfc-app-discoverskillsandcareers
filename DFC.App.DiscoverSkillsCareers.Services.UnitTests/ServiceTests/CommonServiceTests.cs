using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using DFC.App.DiscoverSkillsCareers.Services.SessionHelpers;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.Compui.Sessionstate;
using Dfc.Session.Models;
using FluentAssertions;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class CommonServiceTests
    {
        private readonly ICommonService commonService;
        private readonly ISessionService sessionService;
        private readonly INotificationService notificationService;

        public CommonServiceTests()
        {
            
            sessionService = A.Fake<ISessionService>();
            notificationService = A.Fake<INotificationService>();

            commonService = new CommonService(            
            sessionService,
            notificationService);
        }

        [Fact]
        public void AssessmentServiceSendSmsSendsSms()
        {
            // Arrange
            // Act
            commonService.SendSms("Testdomain.com", "07867511111");

            // Assert
            A.CallTo(() => notificationService.SendSms(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void AssessmentServiceSendEmailSendsEmail()
        {
            // Arrange
            // Act
            commonService.SendEmail("Testdomain.com", "test1@gmail.com");

            // Assert
            A.CallTo(() => notificationService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void AssessmentHasSessionId()
        {
            var sessionId = "session1";

            A.CallTo(() => sessionService.GetCurrentSession()).Returns(new SessionStateModel<DfcUserSession> { State = new DfcUserSession { SessionId = sessionId } });

            // Act
            var result = await commonService.HasSessionStateId();

            // Assert
            result.Should().Be(true);
            A.CallTo(() => sessionService.GetCurrentSession()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void AssessmentHasSessionIdReturnFalse()
        {

            A.CallTo(() => sessionService.GetCurrentSession()).Returns(new SessionStateModel<DfcUserSession> { State = null });

            // Act
            var result = await commonService.HasSessionStateId();

            // Assert
            result.Should().Be(false);
            A.CallTo(() => sessionService.GetCurrentSession()).MustHaveHappenedOnceExactly();
        }
    }
}
