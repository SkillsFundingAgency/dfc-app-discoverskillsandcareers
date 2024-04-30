using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Models;
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

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class AjaxTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly ILogService logService;
        private readonly IDocumentStore documentStore;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

        public AjaxTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            logService = A.Fake<ILogService>();
            documentStore = A.Fake<IDocumentStore>();
            var fakeMemoryCache = A.Fake<IMemoryCache>();
            staticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            cmsApiClientOptions = new CmsApiClientOptions
            {
                ContentIds = Guid.NewGuid().ToString(),
            };

            controller = new ResultsController(logService, mapper, sessionService, resultsService, assessmentService, documentStore, fakeMemoryCache, staticContentDocumentService, cmsApiClientOptions);
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
