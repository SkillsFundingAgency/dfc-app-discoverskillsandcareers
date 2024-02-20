using AutoMapper;
using DFC.App.DiscoverSkillsCareers.GraphQl;
using DFC.App.DiscoverSkillsCareers.MappingProfiles;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using FakeItEasy;
using FluentAssertions;
using Razor.Templating.Core;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.GraphQl
{
    public class GraphQlServiceTests
    {
        [Fact]
        public async Task GetJobProfileAsyncShouldReturnJobProfile()
        {
            // Arrange
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new ResultProfileOverviewsProfile())).CreateMapper();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsync<JobProfileDysacResponse>(A<string>.Ignored)).Returns(A.Dummy<JobProfileDysacResponse>());

            var service = new GraphQlService(fakeSharedContentRedisInterface, fakeRazorTemplateEngine, mapper);

            // Act
            var result = await service.GetJobProfileAsync("test");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<JobProfileViewModel>();
        }
    }
}
