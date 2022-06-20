using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class AssessmentServiceTests
    {
        private readonly ILogger<AssessmentService> logger;
        private readonly NotifyOptions notifyOptions;
        private readonly IAssessmentService assessmentService;
        private readonly ISessionIdToCodeConverter sessionIdToCodeConverter;
        private readonly ISessionService sessionService;
        private readonly IDocumentService<DysacAssessment> assessmentDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> questionSetDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> filteringQuestionDocumentService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;

        public AssessmentServiceTests()
        {
            logger = A.Fake<ILogger<AssessmentService>>();
            notifyOptions = A.Fake<NotifyOptions>();
            sessionIdToCodeConverter = A.Fake<ISessionIdToCodeConverter>();
            sessionService = A.Fake<ISessionService>();
            assessmentDocumentService = A.Fake<IDocumentService<DysacAssessment>>();
            questionSetDocumentService = A.Fake<IDocumentService<DysacQuestionSetContentModel>>();
            mapper = A.Fake<IMapper>();
            filteringQuestionDocumentService = A.Fake<IDocumentService<DysacFilteringQuestionContentModel>>();
            notificationService = A.Fake<INotificationService>();
            var fakeContextAccessor = A.Fake<IHttpContextAccessor>();

            assessmentService = new AssessmentService(
                sessionIdToCodeConverter,
                sessionService,
                assessmentDocumentService,
                questionSetDocumentService,
                filteringQuestionDocumentService,
                mapper,
                notificationService,
                fakeContextAccessor);
        }

        [Fact]
        public async Task NewSessionReturnTrueWhenNewSessionIsCreated()
        {
            var assessmentType = "at1";
            var newAssessmentResponse = new NewSessionResponse() { SessionId = "s1" };

            var response = await assessmentService.NewSession(assessmentType);

            Assert.True(response);
        }

        [Fact]
        public async Task NewSessionCallsCreateCookie()
        {
            var assessmentType = "at1";
            var newAssessmentResponse = new NewSessionResponse() { SessionId = "p1-s1" };

            var result = await assessmentService.NewSession(assessmentType);

            A.CallTo(() => sessionService.CreateCookie(A<string>.Ignored)).MustHaveHappened();
            Assert.True(result);
        }

        [Fact]
        public async Task GetQuestionReturnsValidResponse()
        {
            var sessionId = "session1";
            var assessmentType = "at1";
            var questionNumber = 1;
            var expectedQuestion = new GetQuestionResponse() { NextQuestionNumber = 2 };

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { new DysacAssessment { Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } } } });

            var response = await assessmentService.GetQuestion(assessmentType, questionNumber);
            Assert.Equal(expectedQuestion.NextQuestionNumber, response.NextQuestionNumber);
        }

        [Fact]
        public async Task AnswerQuestionReturnsValidResponse()
        {
            var sessionId = "session1";
            var assessmentType = "at1";
            var questionResponse = new GetQuestionResponse() { QuestionNumber = 1 };
            var answerRequest = new PostAnswerRequest() { QuestionId = $"{assessmentType}-v1-1", SelectedOption = 2 };
            var answerResponse = A.Fake<PostAnswerResponse>();
            answerResponse.IsSuccess = true;

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { new DysacAssessment { Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } } } });

            var response = await assessmentService.AnswerQuestion(assessmentType, questionResponse.QuestionNumber, questionResponse.QuestionNumber, answerRequest.SelectedOption);
            Assert.Equal(answerResponse.IsSuccess, response.IsSuccess);
        }

        [Fact]
        public async Task AnswerFilterQuestionReturnsNextQuestion()
        {
            var sessionId = "session1";
            var jobCategory = "delivery-and-storage";
            var questionResponse = new GetQuestionResponse() { QuestionNumber = 1 };
            var answerRequest = new PostAnswerRequest() { QuestionId = $"{jobCategory}-v1-1", SelectedOption = 2 };
            var answerResponse = A.Fake<PostAnswerResponse>();
            var assessment = new DysacAssessment
            {
                Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } },
                FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { new FilteredAssessmentQuestion { Ordinal = 1, QuestionText = "A question?", TraitCode = "Self Control" }, new FilteredAssessmentQuestion { Ordinal = 2, QuestionText = "Another question?", TraitCode = "Motivation" } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }, LastAnswer = DateTime.Now } } }

            };

            answerResponse.IsSuccess = true;

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });
            A.CallTo(() => questionSetDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel, bool>>>.Ignored)).Returns(new List<DysacQuestionSetContentModel>
            {
                new DysacQuestionSetContentModel
                {
                    ShortQuestions = new List<DysacShortQuestionContentItemModel>()
                }
            });
            
            var response = await assessmentService.AnswerFilterQuestion(jobCategory, questionResponse.QuestionNumber, questionResponse.QuestionNumber, answerRequest.SelectedOption);
            Assert.Equal(answerResponse.IsSuccess, response.IsSuccess);
        }

        [Fact]
        public async Task GetFilteredAssessmentQuestionReturnsQuestion()
        {
            var sessionId = "session1";
            var jobCategory = "delivery-and-storage";
            var answerResponse = A.Fake<PostAnswerResponse>();
            var expectedFilterQuestion = new FilteredAssessmentQuestion { Ordinal = 1, QuestionText = "A question?", TraitCode = "Self Control", Id = Guid.NewGuid() };

            var assessment = new DysacAssessment
            {
                Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } },
                FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { expectedFilterQuestion, new FilteredAssessmentQuestion { Ordinal = 2, QuestionText = "Another question?", TraitCode = "Motivation" } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }, LastAnswer = DateTime.Now } } }

            };

            answerResponse.IsSuccess = true;

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });
            A.CallTo(() => questionSetDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel, bool>>>.Ignored)).Returns(new List<DysacQuestionSetContentModel>
            {
                new DysacQuestionSetContentModel
                {
                    ShortQuestions = new List<DysacShortQuestionContentItemModel>()
                }
            });

            var response = await assessmentService.GetFilteredAssessmentQuestion(jobCategory, 1);

            Assert.Equal(expectedFilterQuestion.QuestionText, response.QuestionText);
            Assert.Equal(expectedFilterQuestion.Ordinal, response.CurrentQuestionNumber);
            Assert.Equal(expectedFilterQuestion.TraitCode, response.TraitCode);
        }

        [Fact]
        public async Task GetAssessmentReturnsAssessment()
        {
            var sessionId = "session1";
            var answerResponse = A.Fake<PostAnswerResponse>();
            var expectedFilterQuestion = new FilteredAssessmentQuestion { Ordinal = 1, QuestionText = "A question?", TraitCode = "Self Control", Id = Guid.NewGuid() };
            var shortQuestion1 = new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() };
            var shortQuestion2 = new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() };

            var assessment = new DysacAssessment
            {
                Questions = new List<ShortQuestion>() { shortQuestion1, shortQuestion2 },
                FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { expectedFilterQuestion, new FilteredAssessmentQuestion { Ordinal = 2, QuestionText = "Another question?", TraitCode = "Motivation" } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }, LastAnswer = DateTime.Now } } }

            };

            answerResponse.IsSuccess = true;

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });
            A.CallTo(() => questionSetDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel, bool>>>.Ignored)).Returns(new List<DysacQuestionSetContentModel>
            {
                new DysacQuestionSetContentModel
                {
                    ShortQuestions = new List<DysacShortQuestionContentItemModel>()
                }
            });

            var response = await assessmentService.GetAssessment();

            Assert.Equal(sessionId, response.SessionId);
            Assert.Equal(shortQuestion1.Id.ToString(), response.QuestionId);
        }
        
        [Fact]
        public async Task GetAssessmentReturnsAssessmentVariation2()
        {
            var sessionId = "session1";
            var answerResponse = A.Fake<PostAnswerResponse>();
            var expectedFilterQuestion = new FilteredAssessmentQuestion { Ordinal = 1, QuestionText = "A question?", TraitCode = "Self Control", Id = Guid.NewGuid() };
            var shortQuestion1 = new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() };
            var shortQuestion2 = new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() };

            var assessment = new DysacAssessment
            {
                Questions = new List<ShortQuestion>() { shortQuestion1, shortQuestion2 },
                FilteredAssessment = new FilteredAssessment { CurrentFilterAssessmentCode = "delivery-and-storage", Questions = new List<FilteredAssessmentQuestion> { expectedFilterQuestion, new FilteredAssessmentQuestion { Ordinal = 2, QuestionText = "Another question?", TraitCode = "Motivation" } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }, LastAnswer = DateTime.Now } } }

            };

            answerResponse.IsSuccess = true;

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });
            A.CallTo(() => questionSetDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel, bool>>>.Ignored)).Returns(new List<DysacQuestionSetContentModel>
            {
                new DysacQuestionSetContentModel
                {
                    ShortQuestions = new List<DysacShortQuestionContentItemModel>()
                }
            });

            var response = await assessmentService.GetAssessment();

            Assert.Equal(sessionId, response.SessionId);
            Assert.Equal(shortQuestion1.Id.ToString(), response.QuestionId);
        }

        [Fact]
        public async Task AssessmentServiceReloadUsingReferenceCodeReloadsAssessment()
        {
            var sessionId = "xE2356Fght3556cv";
            var answerResponse = A.Fake<PostAnswerResponse>();
            var expectedFilterQuestion = new FilteredAssessmentQuestion { Ordinal = 1, QuestionText = "A question?", TraitCode = "Self Control", Id = Guid.NewGuid() };
            var shortQuestion1 = new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() };
            var shortQuestion2 = new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() };

            var assessment = new DysacAssessment
            {
                Questions = new List<ShortQuestion>() { shortQuestion1, shortQuestion2 },
                FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { expectedFilterQuestion, new FilteredAssessmentQuestion { Ordinal = 2, QuestionText = "Another question?", TraitCode = "Motivation" } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }, LastAnswer = DateTime.Now } } }
            };

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });

            var result = await assessmentService.ReloadUsingReferenceCode(sessionId);

            Assert.True(result);
        }

        [Fact]
        public async Task AssessmentServiceReloadUsingSessionIdReloadsAssessment()
        {
            var sessionId = "session1";
            var answerResponse = A.Fake<PostAnswerResponse>();
            var expectedFilterQuestion = new FilteredAssessmentQuestion { Ordinal = 1, QuestionText = "A question?", TraitCode = "Self Control", Id = Guid.NewGuid() };
            var shortQuestion1 = new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() };
            var shortQuestion2 = new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() };

            var assessment = new DysacAssessment
            {
                Questions = new List<ShortQuestion>() { shortQuestion1, shortQuestion2 },
                FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { expectedFilterQuestion, new FilteredAssessmentQuestion { Ordinal = 2, QuestionText = "Another question?", TraitCode = "Motivation" } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }, LastAnswer = DateTime.Now } } }

            };

            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { assessment });

            var result = await assessmentService.ReloadUsingSessionId(sessionId);

            Assert.True(result);
        }

        [Fact]
        public async Task FilterAssessmentCallsFilterAssessmentForCurrentSession()
        {
            var sessionId = "session1";
            var jobCategory = "sports";
            var filterResponse = new FilterAssessmentResponse() { SessionId = sessionId };
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
            A.CallTo(() => filteringQuestionDocumentService.GetAsync(A<Expression<Func<DysacFilteringQuestionContentModel, bool>>>.Ignored)).Returns(new List<DysacFilteringQuestionContentModel>() { new DysacFilteringQuestionContentModel { Id = Guid.NewGuid(), Ordinal = 0, Text = "A question", Skills = new List<DysacSkillContentItemModel> { new DysacSkillContentItemModel { Title = "A skill" } } } });
            A.CallTo(() => assessmentDocumentService.GetAsync(A<Expression<Func<DysacAssessment, bool>>>.Ignored)).Returns(new List<DysacAssessment> { new DysacAssessment { ShortQuestionResult = new ResultData { JobCategories = new List<JobCategoryResult> { new JobCategoryResult { } } }, Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } } } });
            A.CallTo(() => questionSetDocumentService.GetAsync(A<Expression<Func<DysacQuestionSetContentModel, bool>>>.Ignored)).Returns(new List<DysacQuestionSetContentModel>
            {
                new DysacQuestionSetContentModel
                {
                    ShortQuestions = new List<DysacShortQuestionContentItemModel>()
                }
            });
            
            var response = await assessmentService.FilterAssessment(jobCategory);

            A.CallTo(() => assessmentDocumentService.UpsertAsync(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(sessionId, response.SessionId);
        }

        [Fact]
        public void CheckWhetherAReferenceCodeExists()
        {
            var refCode = "dshh88228";

            var response = assessmentService.ReferenceCodeExists(refCode);

            Assert.True(response);
        }

        [Fact]
        public void AssessmentServiceSendSmsSendsSms()
        {
            // Arrange
            // Act
            assessmentService.SendSms("adomain.com", "07867564333");

            // Assert
            A.CallTo(() => notificationService.SendSms(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void AssessmentServiceSendEmailSendsEmail()
        {
            // Arrange
            // Act
            assessmentService.SendEmail("adomain.com", "atest@gmail.com");

            // Assert
            A.CallTo(() => notificationService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
