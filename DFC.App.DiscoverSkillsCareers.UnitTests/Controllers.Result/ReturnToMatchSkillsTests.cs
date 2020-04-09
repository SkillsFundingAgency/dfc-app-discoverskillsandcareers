﻿using System;
using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Controllers;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.UnitTests.Controllers.Result
{
    public class ReturnToMatchSkillsTests
    {
        private readonly ResultsController controller;
        private readonly IMapper mapper;
        private readonly ISessionService sessionService;
        private readonly IAssessmentService assessmentService;
        private readonly IResultsService resultsService;
        private readonly ExternalLinkOptions externalLinkOptions;

        public ReturnToMatchSkillsTests()
        {
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            assessmentService = A.Fake<IAssessmentService>();
            resultsService = A.Fake<IResultsService>();
            externalLinkOptions = new ExternalLinkOptions();

            controller = new ResultsController(mapper, sessionService, resultsService, assessmentService, externalLinkOptions);
        }

        [Fact]
        public void ReturnToMatchSkillsReturnsRedirect()
        {
            // Arrange
            var options = new ExternalLinkOptions();
            var endpoint = options.MatchSkillsResultsEndpoint;
            externalLinkOptions.MatchSkillsResultsEndpoint = options.MatchSkillsResultsEndpoint;

            if (string.IsNullOrWhiteSpace(options.MatchSkillsResultsEndpoint))
            {
                options.MatchSkillsResultsEndpoint = "endpoint";
            }

            // Act
            var actionResponse = controller.ReturnToMatchSkills();

            // Asserts
            Assert.IsType<RedirectResult>(actionResponse);
            Assert.Equal(endpoint,options.MatchSkillsResultsEndpoint);
        }
    }
}

