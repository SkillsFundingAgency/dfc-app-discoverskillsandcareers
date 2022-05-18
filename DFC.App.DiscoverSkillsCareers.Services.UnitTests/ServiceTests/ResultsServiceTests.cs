using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Api;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.Helpers;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using FluentAssertions;
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
        private readonly IResultsService resultsService;
        private readonly ISessionService sessionService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> filteringQuestionDocumentService;
        private readonly IDocumentService<DysacJobProfileCategoryContentModel> jobProfileCategoryDocumentService;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly string sessionId;

        public ResultsServiceTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentDocumentService = A.Fake<IDocumentService<DysacAssessment>>();
            assessmentCalculationService = A.Fake<IAssessmentCalculationService>();
            filteringQuestionDocumentService = A.Fake<IDocumentService<DysacFilteringQuestionContentModel>>();
            jobProfileCategoryDocumentService = A.Fake<IDocumentService<DysacJobProfileCategoryContentModel>>();
            
            resultsService = new ResultsService(sessionService, assessmentCalculationService, assessmentDocumentService,
                filteringQuestionDocumentService,  jobProfileCategoryDocumentService);

            sessionId = "session1";
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
        }

        [Fact]
        public async Task ResultsServiceGetResultsReturnsResults()
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

            //Act
            var results = await resultsService.GetResults();

            //Assert
            A.CallTo(() => assessmentCalculationService.ProcessAssessment(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            results.SessionId.Should().Be(sessionId);
        }

        [Fact]
        public async Task ResultsServiceGetResultsByCategoryReturnsResults()
        {
            //Arrange
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.ShortQuestionResult = new ResultData { JobCategories = new List<JobCategoryResult>() { new JobCategoryResult { JobFamilyName = "delivery and storage", JobProfiles = new List<JobProfileResult> { new JobProfileResult { SkillCodes = new List<string> { "Self Control" } } } } }, Traits = new List<TraitResult>() { new TraitResult { Text = "you enjoy something", TotalScore = 5, TraitCode = "LEADER" } }, JobProfiles = new List<JobProfileResult>(), TraitText = new List<string>() { "you'd be good working in place a", "you might do well in place b", "you're really a at b" } };
            assessment.FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { new FilteredAssessmentQuestion { Ordinal = 0, QuestionText = "A filtered question?", TraitCode = "Self Control", Id = Guid.NewGuid(), Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes } }, new FilteredAssessmentQuestion { Ordinal = 0, QuestionText = "A filtered question 2?", TraitCode = "Self Motivation", Id = Guid.NewGuid(), Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes } } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", LastAnswer = DateTime.MinValue, QuestionSkills = new Dictionary<string, int>() { { "Self Control", 0 } } } } };

            var jobCategory = new DysacJobProfileCategoryContentModel
            {
                JobProfiles = new List<JobProfileContentItemModel>
                {
                    new JobProfileContentItemModel
                    {
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control"
                            }
                        }
                    }
                }
            };
            
            A.CallTo(() => jobProfileCategoryDocumentService.GetAsync(A<Expression<Func<DysacJobProfileCategoryContentModel, bool>>>.Ignored)).Returns(new List<DysacJobProfileCategoryContentModel> { jobCategory });
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });

            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            
            List<JobProfileResult> profiles = new List<JobProfileResult>
            {
                    new JobProfileResult { UrlName = category, JobCategory = category }
            };
            resultsResponse.JobProfiles = profiles;

            List<JobCategoryResult> categories = new List<JobCategoryResult>
             {
                    new JobCategoryResult { JobFamilyName = category, JobFamilyUrl = category }
            };
            resultsResponse.JobCategories = categories;

            //Act
            var results = await resultsService.GetResultsByCategory(category);

            //Assert
            A.CallTo(() => assessmentDocumentService.UpsertAsync(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Single(results.JobCategories);
        }
        
        [Fact]
        public async Task ResultsServiceGetResultsByCategoryWithSkillsReturnsCategoryWithNoProfiles()
        {
            //Arrange
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.ShortQuestionResult = new ResultData { JobCategories = new List<JobCategoryResult>() { new JobCategoryResult { JobFamilyName = "delivery and storage", JobProfiles = new List<JobProfileResult> { new JobProfileResult { SkillCodes = new List<string> { "Self Control", "Another one - that wasnt answered" } } } } }, Traits = new List<TraitResult>() { new TraitResult { Text = "you enjoy something", TotalScore = 5, TraitCode = "LEADER" } }, JobProfiles = new List<JobProfileResult>(), TraitText = new List<string>() { "you'd be good working in place a", "you might do well in place b", "you're really a at b" } };
            assessment.FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { new FilteredAssessmentQuestion { Ordinal = 0, QuestionText = "A filtered question?", TraitCode = "Self Control", Id = Guid.NewGuid(), Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes } }, new FilteredAssessmentQuestion { Ordinal = 0, QuestionText = "A filtered question 2?", TraitCode = "Self Motivation", Id = Guid.NewGuid(), Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes } } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", LastAnswer = DateTime.MinValue, QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } } } } };

            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });
            A.CallTo(() => filteringQuestionDocumentService.GetAsync(A<Expression<Func<DysacFilteringQuestionContentModel, bool>>>.Ignored))
                .Returns(new List<DysacFilteringQuestionContentModel>
                {
                    new DysacFilteringQuestionContentModel
                    {
                        Skills  = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Another one - that wasnt answered"
                            }
                        }
                    }
                }
            );
            var jobCategory = new DysacJobProfileCategoryContentModel
            {
                JobProfiles = new List<JobProfileContentItemModel>
                {
                    new JobProfileContentItemModel
                    {
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Another one - that wasnt answered"
                            }
                        }
                    }
                }
            };
            
            A.CallTo(() => jobProfileCategoryDocumentService.GetAsync(A<Expression<Func<DysacJobProfileCategoryContentModel, bool>>>.Ignored)).Returns(new List<DysacJobProfileCategoryContentModel> { jobCategory });
            
            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            var profiles = new List<JobProfileResult>
            {
                new JobProfileResult { UrlName = category, JobCategory = category, }
            };
            resultsResponse.JobProfiles = profiles;

            var categories = new List<JobCategoryResult>
             {
                new JobCategoryResult { JobFamilyName = category, JobFamilyUrl = category, }
            };
            resultsResponse.JobCategories = categories;

            //Act
            var results = await resultsService.GetResultsByCategory(category);

            //Assert
            A.CallTo(() => assessmentDocumentService.UpsertAsync(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Single(results.JobCategories);
            Assert.Empty(results.JobCategories.Single().JobProfiles);
        }
    }
}
