using AutoMapper;
using DFC.App.DiscoverSkillsCareers.MappingProfiles;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Automapper
{
    public class ResultProfileOverviewsProfileTests
    {

        [Fact]
        public void ResultProfileOverviewsProfileTest()
        {

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ResultProfileOverviewsProfile());
            });

            mappingConfig.AssertConfigurationIsValid();
        }
    }
}
