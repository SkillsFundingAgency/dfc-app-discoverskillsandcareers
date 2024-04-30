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

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class AjaxTests
    {
        private readonly ILogService logService;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;
        private readonly IConfiguration configuration;
        private readonly ResultsController controller;

        public AjaxTests()
        {
            logService = A.Fake<ILogService>();
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            configuration = A.Fake<IConfiguration>();
            razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, sharedContentRedisInterface, razorTemplateEngine, configuration);
        }

        //[Fact]
        //public async Task AjaxChangedCallsUpdateJobCategoryCountsSuccessfully()
        //{
        //    //arranage
        //    var sessionId = await sessionService.GetSessionId().ConfigureAwait(false);
        //    var assessment = await assessmentService.GetAssessment(sessionId).ConfigureAwait(false);

        //    //act
        //    await controller.AjaxChanged().ConfigureAwait(false);

        //    //assert
        //    A.CallTo(() => resultsService.UpdateJobCategoryCounts(assessment).ConfigureAwait(false)).MustHaveHappenedOnceExactly();
        //}
    }
}
