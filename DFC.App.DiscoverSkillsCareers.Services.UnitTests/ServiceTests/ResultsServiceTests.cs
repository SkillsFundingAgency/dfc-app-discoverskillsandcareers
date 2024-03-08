using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.UnitTests.Helpers;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Services;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace DFC.App.DiscoverSkillsCareers.Services.UnitTests.ServiceTests
{
    public class ResultsServiceTests
    {
        private readonly IResultsService resultsService;
        private readonly ISessionService sessionService;
        private readonly IDocumentStore documentStore;
        private readonly IAssessmentCalculationService assessmentCalculationService;
        private readonly IAssessmentService assessmentService;
        private readonly string sessionId;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration = A.Fake<IConfiguration>();

        public ResultsServiceTests()
        {
            sessionService = A.Fake<ISessionService>();
            assessmentCalculationService = A.Fake<IAssessmentCalculationService>();
            assessmentService = A.Fake<IAssessmentService>();
            documentStore = A.Fake<IDocumentStore>();
            var fakeMemoryCache = A.Fake<IMemoryCache>();
            sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();   
            mapper = A.Fake<Mapper>();  
            
            resultsService = new ResultsService(
                sessionService,
                assessmentService,
                assessmentCalculationService, 
                documentStore,
                fakeMemoryCache,
                sharedContentRedisInterface, 
                mapper,
                configuration);

            sessionId = "session1";
            A.CallTo(() => sessionService.GetSessionId()).Returns(sessionId);
        }

        [Fact]
        public async Task ResultsServiceGetResultsReturnsResults()
        {
            //Arrange
            A.CallTo(() => assessmentService.GetAssessment(A<string>.Ignored))
                .Returns(new DysacAssessment { Id = sessionId, Questions = new List<ShortQuestion>() { new ShortQuestion { Ordinal = 0, Id = Guid.NewGuid() }, new ShortQuestion { Ordinal = 1, Id = Guid.NewGuid() } } });

            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            List<JobProfileResult> profiles = new List<JobProfileResult>
            {
                new JobProfileResult()
            };
            resultsResponse.JobProfiles = profiles;

            List<JobCategoryResult> categories = new List<JobCategoryResult>
             {
                    new JobCategoryResult() { JobFamilyName = category, JobFamilyUrl = category }
            };
            resultsResponse.JobCategories = categories;

            //Act
            var results = await resultsService.GetResults(true);

            //Assert
            A.CallTo(() => assessmentCalculationService.ProcessAssessment(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            results.SessionId.Should().Be(sessionId);
        }

        [Fact(Skip = "Further investigation needed")]
        public async Task ResultsServiceGetResultsByCategoryReturnsResults()
        {
            //Arrange
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.ShortQuestionResult = new ResultData
            {
                JobCategories = new List<JobCategoryResult>()
                {
                    new JobCategoryResult
                    {
                        JobFamilyName = "delivery and storage",
                        JobProfiles = new List<JobProfileResult>
                        {
                            new JobProfileResult
                            {
                                Title = "Profile1",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile2",                                
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile3",                                
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile4",                                
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile5",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile6",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile7",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile8",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5",
                                    "Self Control"
                                }
                            }
                        }
                    }
                },
                Traits = new List<TraitResult>
                {
                    new TraitResult
                    {
                        Text = "you enjoy something", TotalScore = 5, TraitCode = "LEADER"
                    }
                },
                TraitText = new List<string>
                {
                    "you'd be good working in place a", "you might do well in place b", "you're really a at b"
                }
            };
            
            assessment.FilteredAssessment = new FilteredAssessment
            {
                Questions = new List<FilteredAssessmentQuestion>
                {
                    new FilteredAssessmentQuestion
                    {
                        Ordinal = 0,
                        QuestionText = "A filtered question?",
                        TraitCode = "Self Control",
                        Id = Guid.NewGuid(),
                        Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes }
                    },
                    new FilteredAssessmentQuestion
                    {
                        Ordinal = 0,
                        QuestionText = "A filtered question 2?",
                        TraitCode = "Self Motivation",
                        Id = Guid.NewGuid(),
                        Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes }
                    }
                },
                JobCategoryAssessments = new List<JobCategoryAssessment>
                {
                    new JobCategoryAssessment
                    {
                        JobCategory = "delivery-and-storage",
                        LastAnswer = DateTime.MinValue,
                        QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }
                    }
                }
            };

            var jobCategory = new DysacJobProfileCategoryContentModel
            {
                JobProfiles = new List<JobProfileContentItemModel>
                {
                    new JobProfileContentItemModel
                    {
                        Title = "Profile1",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile2",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile3",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile4",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile5",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile6",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile7",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile8",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control"
                            }
                        }
                    }
                }
            };
            
            A.CallTo(() => documentStore.GetAllContentAsync<DysacJobProfileCategoryContentModel>("JobProfileCategory", A<string>.Ignored))
                .Returns(new List<DysacJobProfileCategoryContentModel> { jobCategory });
            A.CallTo(() => assessmentService.GetAssessment(A<string>.Ignored)).Returns(assessment);

            var category = "ACategory";
            var resultsResponse = new GetResultsResponse { SessionId = sessionId };
            
            List<JobProfileResult> profiles = new List<JobProfileResult>
            {
                new JobProfileResult()
            };
            resultsResponse.JobProfiles = profiles;

            List<JobCategoryResult> categories = new List<JobCategoryResult>
            {
                new JobCategoryResult { JobFamilyName = category, JobFamilyUrl = category }
            };
            resultsResponse.JobCategories = categories;

            //Act
            var results = await resultsService.GetResultsByCategory(category);

            // Assert
            A.CallTo(() => assessmentService.UpdateAssessment(A<DysacAssessment>.Ignored))
                .MustHaveHappenedOnceExactly();
            
            Assert.Single(results.JobCategories);
        }

        [Fact(Skip = "Further investigation needed")]
        public async Task ResultsServiceGetResultsByCategoryWithSkillsReturnsCategoryWithSingleProfile()
        {
            //Arrange
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.ShortQuestionResult = new ResultData { JobCategories = new List<JobCategoryResult>() { new JobCategoryResult { JobFamilyName = "delivery and storage", JobProfiles = new List<JobProfileResult> { new JobProfileResult { SkillCodes = new List<string> { "Self Control", "Another one - that wasnt answered" } } } } }, Traits = new List<TraitResult>() { new TraitResult { Text = "you enjoy something", TotalScore = 5, TraitCode = "LEADER" } }, TraitText = new List<string>() { "you'd be good working in place a", "you might do well in place b", "you're really a at b" } };
            assessment.FilteredAssessment = new FilteredAssessment { Questions = new List<FilteredAssessmentQuestion> { new FilteredAssessmentQuestion { Ordinal = 0, QuestionText = "A filtered question?", TraitCode = "Self Control", Id = Guid.NewGuid(), Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes } }, new FilteredAssessmentQuestion { Ordinal = 0, QuestionText = "A filtered question 2?", TraitCode = "Self Motivation", Id = Guid.NewGuid(), Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes } } }, JobCategoryAssessments = new List<JobCategoryAssessment> { new JobCategoryAssessment { JobCategory = "delivery-and-storage", LastAnswer = DateTime.MinValue, QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } } } } };

            A.CallTo(() => assessmentService.GetAssessment(A<string>.Ignored)).Returns(assessment);
            A.CallTo(() => documentStore.GetAllContentAsync<DysacFilteringQuestionContentModel>("FilteringQuestion", A<string>.Ignored))
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
            
            A.CallTo(() => documentStore.GetAllContentAsync<DysacJobProfileCategoryContentModel>("JobProfileCategory", A<string>.Ignored))
                .Returns(new List<DysacJobProfileCategoryContentModel> { jobCategory });
            
            var category = "ACategory";
            var resultsResponse = new GetResultsResponse() { SessionId = sessionId };
            var profiles = new List<JobProfileResult>
            {
                new JobProfileResult()
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
            A.CallTo(() => assessmentService.UpdateAssessment(A<DysacAssessment>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Single(results.JobCategories);
            Assert.Single(results.JobCategories.Single().JobProfiles);
        }

        [Fact(Skip ="Further investigation needed")]
        public async Task GetResultsByCategory_ReturnsNotAllJobProfilesMatchingPropertyAsTrue_WhenNotAllAssessmentJobProfilesMatchWithStaxJobProfiles()
        {
            //Arrange
            var assessment = AssessmentHelpers.GetAssessment();
            assessment.ShortQuestionResult = new ResultData
            {
                JobCategories = new List<JobCategoryResult>()
                {
                    new JobCategoryResult
                    {
                        JobFamilyName = "delivery and storage",
                        JobProfiles = new List<JobProfileResult>
                        {
                            new JobProfileResult
                            {
                                Title = "Profile19",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile2",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile3",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile4",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile5",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile6",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile7",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5"
                                }
                            },
                            new JobProfileResult
                            {
                                Title = "Profile8",
                                SkillCodes = new List<string>
                                {
                                    "Self Control1",
                                    "Self Control2",
                                    "Self Control3",
                                    "Self Control4",
                                    "Self Control5",
                                    "Self Control"
                                }
                            }
                        }
                    }
                },
                Traits = new List<TraitResult>
                {
                    new TraitResult
                    {
                        Text = "you enjoy something", TotalScore = 5, TraitCode = "LEADER"
                    }
                },
                TraitText = new List<string>
                {
                    "you'd be good working in place a", "you might do well in place b", "you're really a at b"
                }
            };

            assessment.FilteredAssessment = new FilteredAssessment
            {
                Questions = new List<FilteredAssessmentQuestion>
                {
                    new FilteredAssessmentQuestion
                    {
                        Ordinal = 0,
                        QuestionText = "A filtered question?",
                        TraitCode = "Self Control",
                        Id = Guid.NewGuid(),
                        Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes }
                    },
                    new FilteredAssessmentQuestion
                    {
                        Ordinal = 0,
                        QuestionText = "A filtered question 2?",
                        TraitCode = "Self Motivation",
                        Id = Guid.NewGuid(),
                        Answer = new QuestionAnswer { AnsweredAt = DateTime.Now, Value = Answer.Yes }
                    }
                },
                JobCategoryAssessments = new List<JobCategoryAssessment>
                {
                    new JobCategoryAssessment
                    {
                        JobCategory = "delivery-and-storage",
                        LastAnswer = DateTime.MinValue,
                        QuestionSkills = new Dictionary<string, int> { { "Self Control", 0 } }
                    }
                }
            };

            var jobCategory = new DysacJobProfileCategoryContentModel
            {
                JobProfiles = new List<JobProfileContentItemModel>
                {
                    new JobProfileContentItemModel
                    {
                        Title = "Profile1",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile2",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile3",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile4",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile5",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile6",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile7",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            }
                        }
                    },
                    new JobProfileContentItemModel
                    {
                        Title = "Profile8",
                        Skills = new List<DysacSkillContentItemModel>
                        {
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control1"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control2"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control3"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control4"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control5"
                            },
                            new DysacSkillContentItemModel
                            {
                                Title = "Self Control"
                            }
                        }
                    }
                }
            };

            A.CallTo(() => documentStore.GetAllContentAsync<DysacJobProfileCategoryContentModel>("JobProfileCategory", A<string>.Ignored))
                .Returns(new List<DysacJobProfileCategoryContentModel> { jobCategory });
            A.CallTo(() => assessmentService.GetAssessment(A<string>.Ignored)).Returns(assessment);

            var category = "ACategory";
            var resultsResponse = new GetResultsResponse { SessionId = sessionId };

            List<JobProfileResult> profiles = new List<JobProfileResult>
            {
                new JobProfileResult()
            };
            resultsResponse.JobProfiles = profiles;

            List<JobCategoryResult> categories = new List<JobCategoryResult>
            {
                new JobCategoryResult { JobFamilyName = category, JobFamilyUrl = category }
            };
            resultsResponse.JobCategories = categories;

            //Act
            var results = await resultsService.GetResultsByCategory(category);

            // Assert
            A.CallTo(() => assessmentService.UpdateAssessment(A<DysacAssessment>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.False(results.AllJobProfilesMatchWithAssessmentProfiles);
        }
    }
}
