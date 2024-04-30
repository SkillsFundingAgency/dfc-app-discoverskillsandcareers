using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Moq;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using Razor.Templating.Core;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class AjaxTests
    {
        private readonly ILogService FakeLogService;
        private readonly IMapper FakeMapper;
        private readonly ISessionService FakeSessionService;
        private readonly IAssessmentService FakeAssessmentService;
        private readonly IResultsService FakeResultsService;
        private readonly ISharedContentRedisInterface FakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine FakeRazorTemplateEngine;
        private readonly IConfiguration FakeConfiguration;
        private readonly ResultsController controller;

        public AjaxTests()
        {
            FakeLogService = A.Fake<ILogService>();
            FakeMapper = A.Fake<IMapper>();
            FakeSessionService = A.Fake<ISessionService>();
            FakeAssessmentService = A.Fake<IAssessmentService>();
            FakeResultsService = A.Fake<IResultsService>();
            FakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            FakeConfiguration = A.Fake<IConfiguration>();
            FakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            controller = new ResultsController(FakeLogService, FakeMapper, FakeSessionService, FakeResultsService, FakeAssessmentService, FakeSharedContentRedisInterface, FakeRazorTemplateEngine, FakeConfiguration);
        }

        [Fact]
        public async Task AjaxChangedCallsUpdateJobCategoryCountsSuccessfully()
        {
            //act
            await controller.AjaxChanged().ConfigureAwait(false);

            //assert
            A.CallTo(() => FakeResultsService.UpdateJobCategoryCounts(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
