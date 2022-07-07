using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using FakeItEasy;
using Notify.Interfaces;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.NotificationServiceTests
{
    public class NotificationServiceTests
    {
        private readonly INotificationClient notificationClient = A.Fake<INotificationClient>();
        private readonly NotifyOptions notificationOptions = A.Fake<NotifyOptions>();

        [Fact]
        public void NotificationServiceSendEmailReturnsTrue()
        {
            // Arrange
            var serviceToTest = new NotificationService(notificationClient, notificationOptions);

            // Act
            var result = serviceToTest.SendEmail("somedomain.com", "test@gmail.com", "abcdefg1234213", "abcdefg1234213");

            // Assert
            A.CallTo(() => notificationClient.SendEmail(A<string>.Ignored, A<string>.Ignored, A<Dictionary<string, dynamic>>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.True(result.IsSuccess);
        }


        [Fact]
        public void NotificationServiceSendSmsReturnsTrue()
        {
            // Arrange
            var serviceToTest = new NotificationService(notificationClient, notificationOptions);

            // Act
            var result = serviceToTest.SendSms("somedomain.com", "07965463099", "abcdefg1234213","abcdefg1234213");

            // Assert
            A.CallTo(() => notificationClient.SendSms(A<string>.Ignored, A<string>.Ignored, A<Dictionary<string, dynamic>>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.True(result.IsSuccess);
        }
    }
}
