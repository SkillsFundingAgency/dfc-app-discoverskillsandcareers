using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Collections.Generic;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.DataProcessors;
using FakeItEasy;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.Helpers
{
    public static class GeneralHelperTests
    {
        [Theory]
        [InlineData("ABCDE-1234", "1234")]
        [InlineData("5678", "5678")]
        [InlineData("AB-1234", "AB-1234")]
        [InlineData("ABCDEFG-1234", "ABCDEFG-1234")]        
        public static void GetGenericSkillNameStripsFirstPartIfRequired(string input, string expected)
        {
            var actual = GeneralHelper.GetGenericSkillName(input);
            Assert.Equal(expected, actual);
        }
    }
}
