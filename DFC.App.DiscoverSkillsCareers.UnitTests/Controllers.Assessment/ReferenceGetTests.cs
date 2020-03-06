using DFC.App.DiscoverSkillsCareers.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Assessment
{
    public class ReferenceGetTests : AssessmentTestBase
    {
        [Fact]
        public async Task NullViewModelReturnsBadRequest()
        {
            var actionResponse = await AssessmentController.Reference().ConfigureAwait(false);
            Assert.IsType<ViewResult>(actionResponse);
        }
    }
}
