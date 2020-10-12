using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ResultsServiceTests
    {
        private readonly IResultsApiService resultsApiService;
        private readonly IJpOverviewApiService jPOverviewAPIService;
        private readonly ISessionService sessionService;
        private readonly IResultsService resultsService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly string sessionId;

        public ResultsServiceTests()
        {
            resultsApiService = A.Fake<IResultsApiService>();
            sessionService = A.Fake<ISessionService>();
            jPOverviewAPIService = A.Fake<IJpOverviewApiService>();
            assessmentDocumentService = A.Fake<IDocumentService<DysacAssessment>>();
            assessmentCalculationService = A.Fake<IAssessmentCalculationService>();
            resultsService = new ResultsService(sessionService, assessmentCalculationService, assessmentDocumentService);

            sessionId = "session1";
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
        }

        [Fact]
        public async Task GetResults()
        {
            //Arrange
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { new DysacAssessment { AssessmentCode = sessionId, Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } } } });

            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            List<JobProfileResult> profiles = new List<JobProfileResult>
            {
                    new JobProfileResult() { UrlName = category, JobCategory = category }
            };
            resultsResponse.JobProfiles = profiles;

            List<JobCategoryResult> categories = new List<JobCategoryResult>
             {
                    new JobCategoryResult() { JobFamilyName = category, JobFamilyUrl = category }
            };
            resultsResponse.JobCategories = categories;

            A.CallTo(() => resultsApiService.GetResults(sessionId, A<string>.Ignored)).Returns(resultsResponse);

            //Act
            var results = await resultsService.GetResults();

            //Assert
            A.CallTo(() => assessmentCalculationService.ProcessAssessment(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            results.SessionId.Should().Be(sessionId);
        }

        //[Theory]
        //[InlineData(false, 0)]
        //[InlineData(true, 1)]
        //public async Task GetResultsByCategory(bool hasMatchedProfile, int expectedNumberOfcalls)
        //{
        //    //Arrange
        //    A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { new DysacAssessment { AssessmentCode = sessionId, Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } } } });

        //    var category = "ACategory";
        //    var resultsResponse = new GetResultsResponse() { SessionId = sessionId };

        //    if (hasMatchedProfile)
        //    {
        //        List<JobProfileResult> profiles = new List<JobProfileResult>
        //        {
        //            new JobProfileResult() { UrlName = "Cname1", JobCategory = category }
        //        };
        //        resultsResponse.JobProfiles = profiles;

        //        List<JobCategoryResult> categories = new List<JobCategoryResult>
        //        {
        //            new JobCategoryResult() { JobFamilyName = category, JobFamilyUrl = category }
        //        };
        //        resultsResponse.JobCategories = categories;
        //    }

        //    A.CallTo(() => resultsApiService.GetResults(sessionId, A<string>.Ignored)).Returns(resultsResponse);
            
        //    //Act
        //    var results = await resultsService.GetResultsByCategory(category);

        //    //Assert
        //    A.CallTo(() => resultsApiService.GetResults(sessionId, category)).MustHaveHappenedOnceExactly();
        //    results.SessionId.Should().Be(sessionId);
        //}

    }
}
