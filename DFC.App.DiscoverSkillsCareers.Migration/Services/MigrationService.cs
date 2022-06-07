using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Core.Enums;
using DFC.App.DiscoverSkillsCareers.Migration.Models;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using Microsoft.Azure.Documents.Linq;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IDocumentService<DysacTraitContentModel> dysacTraitDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;
        private readonly IDocumentClient sourceDocumentClient;
        private readonly IDocumentClient destinationDocumentClient;
        
        private List<JobCategoryContentItemModel> allJobCategories = new List<JobCategoryContentItemModel>();
        private List<DysacFilteringQuestionContentModel> filteringQuestions = new List<DysacFilteringQuestionContentModel>();
        private List<ShortQuestion> shortQuestions = new List<ShortQuestion>();
        private readonly StringBuilder logger = new StringBuilder();
        private string destinationDatabaseId;
        private string destinationCollectionId;
        private int saves = 0;
        
        public MigrationService(
            IDocumentService<DysacTraitContentModel> dysacTraitDocumentService,
            IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService,
            IDocumentClient sourceDocumentClient,
            IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService,
            IDocumentClient destinationDocumentClient,
            string destinationDatabaseId,
            string destinationCollectionId)
        {
            this.dysacTraitDocumentService = dysacTraitDocumentService;
            this.dysacFilteringQuestionDocumentService = dysacFilteringQuestionDocumentService;
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
            this.sourceDocumentClient = sourceDocumentClient;
            this.destinationDocumentClient = destinationDocumentClient;
            this.destinationDatabaseId = destinationDatabaseId;
            this.destinationCollectionId = destinationCollectionId;
        }

        public async Task Start()
        {
            var errors = new List<string>();

            try
            {
                var sessionsToMigrateTask = GetSessionIdentifiersToMigrate();

                await Task.WhenAll(
                    LoadJobCategoriesFromTraits(),
                    LoadFilteringQuestions(),
                    LoadShortQuestionsFromQuestionSet());

                var sessionsToMigrate = await sessionsToMigrateTask;

                var sessionsToMigrateCount = sessionsToMigrate.Count;
                var index = 1;

                const int readBatchSize = 1000;
                const int
                    writeBatchSize = 100; // 4 at 400, 10 at 1000, 100 at 10,000, 400 at 40,000 - max RU is around 99

                var sessionGroups = sessionsToMigrate.Batch(readBatchSize);
                var sessionGroupsCount = sessionGroups.Count();

                var outerForeachCount = 1;

                foreach (var sessionGroup in sessionGroups)
                {
                    var sessionIds = sessionGroup.ToList();

                    var legacySessions = await LoadLegacySessions(sessionIds, outerForeachCount, sessionGroupsCount);
                    var legacySessionsWriteGroups = legacySessions.Batch(writeBatchSize);

                    foreach (var legacySessionWriteGroup in legacySessionsWriteGroups)
                    {
                        var sessionWriteGroupStartTime = DateTime.Now;
                        var createTasks = new List<Task>();

                        foreach (var session in legacySessionWriteGroup)
                        {
                            var sessionId = (string) session["id"];
                            Log(
                                $"Started processing assessment {index} of {sessionsToMigrateCount} ({sessionId}) at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");

                            try
                            {
                                var migratedAssessment = new DysacAssessmentForCreate
                                {
                                    Id = sessionId
                                };

                                var recordedAnswers = ((session["assessmentState"] as JObject)!
                                        .ToObject<Dictionary<string, object>>()!
                                        ["recordedAnswers"] as JArray)!
                                    .Select(x => x.ToObject<Dictionary<string, object>>())
                                    .ToList();

                                migratedAssessment.Questions = ConvertToQuestions(recordedAnswers);

                                migratedAssessment.ShortQuestionResult = ConvertToShortQuestionResult(
                                    (session["resultData"] as JObject)?.ToObject<Dictionary<string, object>>());

                                migratedAssessment.FilteredAssessment = ConvertToFilteredAssessment(
                                    (session["filteredAssessmentState"] as JObject)
                                    ?.ToObject<Dictionary<string, object>>(),
                                    migratedAssessment.ShortQuestionResult!);

                                createTasks.Add(Create(migratedAssessment, index, sessionsToMigrateCount));
                            }
                            catch (Exception exception)
                            {
                                Log($"Error processing {index} of {legacySessions.Count} - {exception.Message} " +
                                    $"at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");

                                errors.Add(sessionId);
                            }
                            finally
                            {
                                index++;
                            }
                        }

                        await Task.WhenAll(createTasks);

                        var duration = DateTime.Now - sessionWriteGroupStartTime;

                        if (duration.TotalSeconds < 1 && createTasks.Any())
                        {
                            var remainder = (int) ((1.0 - duration.TotalSeconds) * 1000);

                            Log($"Too quick. Waiting {remainder}ms at {DateTime.Now:yyyy-MM-dd hh:mm:ss}.");

                            // Too quick, waiting remaining time
                            await Task.Delay(remainder);
                        }

                        Log($"Finished upserting batch of {writeBatchSize} documents at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
                    }

                    outerForeachCount += 1;
                }

                Log($"Completed, with {errors.Count} errors at {DateTime.Now:yyyy-MM-dd hh:mm:ss}.");
                Log(string.Empty);
                Log(string.Empty);
            }
            catch (Exception ex)
            {
                var msg = "Fatal error: " + ex.Message + " - " + ex.StackTrace;
                Log(msg);

                errors.Add(msg);
                Log(string.Empty);
            }
            finally
            {
                Log("Results:");
                Log($"{saves} items moved.");
                Log(string.Empty);
                Log(string.Empty);

                Log("Error summary:");
            
                foreach (var error in errors)
                {
                    Log($"Error - {error}");                                
                }
            
                File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd")}-report.txt", logger.ToString());   
            }
        }
        
        private void Log(string message)
        {
            Console.WriteLine(message);
            logger.AppendLine(message);
        }

        private async Task Create(DysacAssessmentForCreate migratedAssessment, int index, int count)
        {
            Log($"Started creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var resourceResponse = await destinationDocumentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(destinationDatabaseId, destinationCollectionId),
                migratedAssessment,
                new RequestOptions());

            saves += 1;
            var charge = resourceResponse.RequestCharge;
            
            Log($"Finished creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds. Charge was {charge} RUs");
        }

        private async Task LoadJobCategoriesFromTraits()
        {
            Log($"Started fetching all job categories from traits - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var allTraits = await dysacTraitDocumentService
                .GetAsync(document => document.PartitionKey == "Trait");

            allJobCategories = allTraits?
                .SelectMany(trait => trait.JobCategories)
                .GroupBy(jobCategory => jobCategory.Title)
                .Select(jobCategoryGroup => jobCategoryGroup.First())
                .ToList();
            
            Log($"Finished fetching all job categories from traits - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
        }
        
        private async Task LoadFilteringQuestions()
        {
            Log($"Started fetching filtering questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            filteringQuestions = (await dysacFilteringQuestionDocumentService
                .GetAsync(document => document.PartitionKey == "FilteringQuestion"))!
                .ToList();
            
            Log($"Finished fetching filtering questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
        }
        
        private async Task LoadShortQuestionsFromQuestionSet()
        {
            Log($"Started fetching short questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var questionSets = await dysacQuestionSetDocumentService
                .GetAsync(document => document.PartitionKey == "QuestionSet");

            shortQuestions = questionSets?
                .FirstOrDefault()?
                .ShortQuestions?
                .Select(shortQuestion => new ShortQuestion
                {
                    Id = shortQuestion.ItemId,
                    QuestionText = shortQuestion.Title,
                    IsNegative = shortQuestion.Impact != "Positive",
                    Ordinal = shortQuestion.Ordinal,
                    Answer = null,
                    Trait = shortQuestion.Traits.Single().Title
                })
                .ToList();
            
            Log($"Finished fetching short questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");            
        }
        
        private async Task<List<string>> GetSessionIdentifiersToMigrate()
        {
            Log($"Started fetching all session identifiers - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;

            var hasMore = true;
            var returnList = new List<string>();
            var loopCount = 0;
            var countToFetch = 10000;
            var maxLoops = 100;
            
            while (hasMore && loopCount < maxLoops)
            {
                var query = sourceDocumentClient.CreateDocumentQuery<int>(
                    UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                    $"select c.id from c order by c._ts asc OFFSET {countToFetch * loopCount} LIMIT {countToFetch}",
                    new FeedOptions
                    {
                        EnableCrossPartitionQuery = true,
                        MaxItemCount = -1
                    }).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    returnList.AddRange((await query.ExecuteNextAsync<SessionIdentifier>()).Select(sessionIdentifier => sessionIdentifier.Id).ToList());
                }

                hasMore = returnList.Count % countToFetch == 0;
                loopCount++;
                
                Log($"Fetched {returnList.Count} session identifiers in total. Continuing... - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            }

            Log($"Finished fetching all session identifiers. Found {returnList.Count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - " + 
                $"took {(DateTime.Now - start).TotalSeconds} seconds");

            return returnList;
        }
        
        private async Task<List<Dictionary<string, object>>> LoadLegacySessions(List<string> sessionIds,int outerForeachCount, int sessionGroupsCount)
        {
            Log($"Started fetching legacy session identifiers for batch of {sessionIds.Count} - {outerForeachCount} of {sessionGroupsCount} - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var query = sourceDocumentClient.CreateDocumentQuery<int>(
                UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                new SqlQuerySpec(
                    "select c from c where ARRAY_CONTAINS(@idList0, c.id)",
                    new SqlParameterCollection
                    {
                        new SqlParameter
                        {
                            Name = "@idList0",
                            Value = sessionIds.ToArray()
                        }
                    }),
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = -1
                }).AsDocumentQuery();

            var returnList = new List<Dictionary<string, object>>();
            
            while (query.HasMoreResults)
            {
                returnList.AddRange((await query.ExecuteNextAsync<Dictionary<string, object>>())
                    .Select(dyn => (dyn["c"] as JObject)!.ToObject<Dictionary<string, object>>())
                    .ToList());
            }

            Log($"Finished fetching legacy session identifiers for batch. Found {returnList.Count} - " +
                $"{DateTime.Now:yyyy-MM-dd hh:mm:ss} - took {(DateTime.Now - start).TotalSeconds} seconds");

            return returnList;
        }

        private FilteredAssessment ConvertToFilteredAssessment(Dictionary<string, object> filteredAssessmentState, ResultData resultData)
        {
            if (filteredAssessmentState == null)
            {
                return null;
            }
            
            var jobCategories = (filteredAssessmentState["jobCategories"] as JArray)!
                .Select(jobCategory => jobCategory.ToObject<Dictionary<string, object>>())
                .ToList();

            var jobCategoryMappings = resultData.JobCategories!
                .Select(jobCategory =>
                    new KeyValuePair<string, string>(jobCategory.JobFamilyName, jobCategory.JobFamilyNameUrl))
                .ToDictionary(
                    jobCategoryKvp => jobCategoryKvp.Key,
                    jobCategoryKvp => jobCategoryKvp.Value);
            
            var filteredAssessment = new FilteredAssessment
            {
                JobCategoryAssessments = AddJobCategoryAssessments(jobCategories, jobCategoryMappings)
            };
            
            filteredAssessment.Questions = AddFilterQuestions(filteredAssessment.JobCategoryAssessments);

            var recordedAnswers = (filteredAssessmentState["recordedAnswers"] as JArray)!
                .Select(jobCategory => jobCategory.ToObject<Dictionary<string, object>>())
                .ToList();
            
            AddFilterAnswers(recordedAnswers, filteredAssessment.Questions.ToList());

            var code = (string) filteredAssessmentState["currentFilterAssessmentCode"];

            if (!string.IsNullOrEmpty(code))
            {
                filteredAssessment.CurrentFilterAssessmentCode = resultData.JobCategories
                    .First(jc => jc.JobFamilyCode!.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                    .JobFamilyUrl;
            }

            return filteredAssessment;
        }

        private static void AddFilterAnswers(List<Dictionary<string, object>> recordedAnswers, List<FilteredAssessmentQuestion> questions)
        {
            foreach (var recordedAnswer in recordedAnswers)
            {
                var answerTraitAsString = (string)recordedAnswer["traitCode"];
                var answeredDate = (DateTime)recordedAnswer["answeredDt"];
                var selectedAnswer = (Answer)(long)recordedAnswer["selectedOption"];

                questions
                    .First(question => string.Equals(question.TraitCode!, answerTraitAsString, StringComparison.InvariantCultureIgnoreCase))!
                    .Answer = new QuestionAnswer { AnsweredAt = answeredDate, Value = selectedAnswer };
            }
        }

        private IEnumerable<FilteredAssessmentQuestion> AddFilterQuestions(IEnumerable<JobCategoryAssessment> jobCategoryAssessments)
        {
            var distinctGroupedQuestions = jobCategoryAssessments
                .SelectMany(jobCategoryAssessment => jobCategoryAssessment.QuestionSkills)
                .GroupBy(questionSkills => questionSkills.Key)
                .Select(questionSkillGroup => questionSkillGroup.First());

            var filteredAssessmentQuestions = new List<FilteredAssessmentQuestion>();
            var questionIndex = 0;
            
            foreach (var question in distinctGroupedQuestions)
            {
                var filterQuestion = filteringQuestions
                    .FirstOrDefault(filteringQuestion =>
                        string.Equals(filteringQuestion.Title!, question.Key, StringComparison.InvariantCultureIgnoreCase));

                if (filterQuestion == null)
                {
                    if (question.Key != "Persistence")
                    {
                        throw new InvalidOperationException(
                            $"Filter question {question.Key} not found in Stax Filter Question Repository");
                    }
                }

                filteredAssessmentQuestions.Add(new FilteredAssessmentQuestion
                {
                    Id = filterQuestion?.Id ?? Guid.NewGuid(),
                    Ordinal = questionIndex++,
                    QuestionText = filterQuestion?.Text,
                    TraitCode = question.Key
                });
            }

            return filteredAssessmentQuestions;
        }

        private List<JobCategoryAssessment> AddJobCategoryAssessments(
            List<Dictionary<string, object>> jobCategories,
            Dictionary<string, string> jobCategoryMappings)
        {
            var jobCategoryAssessments = new List<JobCategoryAssessment>();

            foreach (var jobCategory in jobCategories)
            {
                var jobCategoryAssessment = new JobCategoryAssessment();
                var questions = (jobCategory["questions"] as JArray)!
                    .Select(x => x.ToObject<Dictionary<string, object>>())
                    .ToList();

                foreach (var question in questions)
                {
                    var jobCategoryName = (string)jobCategory["jobCategoryName"];
                    jobCategoryAssessment.JobCategory = jobCategoryMappings[jobCategoryName];

                    var skill = (string)question["Skill"];
                    var skillQuestion = filteringQuestions
                        .FirstOrDefault(filteringQuestion =>
                            string.Equals(filteringQuestion.Title!, skill, StringComparison.InvariantCultureIgnoreCase));

                    if (skillQuestion == null)
                    {
                        if (skill != "Persistence")
                        {
                            throw new InvalidOperationException($"Filter question {skill} not found in Stax Filter Question Repository");
                        }
                    }

                    jobCategoryAssessment.QuestionSkills.Add(skill, skillQuestion?.Skills.First().Ordinal.Value ?? 0);
                    jobCategoryAssessments.Add(jobCategoryAssessment);
                }
            }

            return jobCategoryAssessments;
        }

        private ResultData ConvertToShortQuestionResult(Dictionary<string, object> resultData)
        {
            var returnItem = new ResultData();

            BuildTraits(resultData, returnItem);
            BuildJobCategories(resultData, returnItem);

            if (returnItem.JobCategories != null || returnItem.JobProfiles != null)
            {
                return returnItem;
            }

            return null;
        }

        private void BuildJobCategories(Dictionary<string, object> resultDataDictionary, ResultData resultData)
        {
            if (resultDataDictionary == null)
            {
                return;
            }
            
            var jobCategories = (resultDataDictionary["jobFamilies"] as JArray)!
                .Select(x => x.ToObject<Dictionary<string, object>>())
                .ToList();
            
            var returnJobCategories = new List<JobCategoryResult>();

            foreach (var jobCategory in jobCategories)
            {
                // Get the job profile and the skills
                JobCategoryResult categoryToAdd = JsonConvert.DeserializeObject<JobCategoryResult>(JsonConvert.SerializeObject(jobCategory));
                
                categoryToAdd.JobProfiles = GetJobProfilesAndSkills(categoryToAdd.JobFamilyNameUrl);
                categoryToAdd.SkillQuestions = categoryToAdd.JobProfiles
                    .SelectMany(jobProfile => jobProfile.SkillCodes)
                    .GroupBy(skillCode => skillCode)
                    .Select(skillCodeGroup => skillCodeGroup.First());
                categoryToAdd.Total = categoryToAdd.SkillQuestions.Count();
                    
                returnJobCategories.Add(categoryToAdd);
            }

            resultData.JobCategories = returnJobCategories;
        }

        private IEnumerable<JobProfileResult> GetJobProfilesAndSkills(string jobFamilyNameUrl)
        {
            var resultsToReturn = new List<JobProfileResult>();

            var category = allJobCategories.FirstOrDefault(jobCategory =>
                jobCategory.Title!.Replace(" ", "-").Replace(",", string.Empty)
                    .Equals(jobFamilyNameUrl, StringComparison.InvariantCultureIgnoreCase));

            if (category == null)
            {
                return resultsToReturn;
            }

            // Old DYSAC, "Computing, Technology and Digital" is one category, needs sorting.
            foreach (var profile in category.JobProfiles)
            {
                resultsToReturn.Add(new JobProfileResult
                {
                    Title = profile.Title,
                    SkillCodes = profile.Skills.Select(skill => skill.Title).ToList()
                });
            }
            
            return resultsToReturn;
        }

        private static void BuildTraits(Dictionary<string, object> resultDataDictionary, ResultData resultData)
        {
            if (resultDataDictionary == null)
            {
                return;
            }
            
            var traits = (resultDataDictionary["traits"] as JArray)!
                .Select(x => x.ToObject<Dictionary<string, object>>())
                .ToList();
            
            var returnTraits = new List<TraitResult>();

            foreach (var trait in traits)
            {
                returnTraits.Add(new TraitResult
                {
                    Text = (string)trait["traitText"],
                    TotalScore = Convert.ToInt32((long)trait["totalScore"]),
                    TraitCode = (string)trait["traitCode"]
                });
            }

            resultData.Traits = returnTraits;
        }

        private IEnumerable<ShortQuestion> ConvertToQuestions(List<Dictionary<string, object>> recordedAnswers)
        {
            var listOfQuestions = new List<ShortQuestion>();

            foreach (var recordedAnswer in recordedAnswers)
            {
                var shortQuestion = new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionText = (string)recordedAnswer["questionText"],
                    Ordinal = Convert.ToInt32((long)recordedAnswer["questionNumber"] - 1),
                    IsNegative = (bool)recordedAnswer["isNegative"],
                    Trait = (string)recordedAnswer["traitCode"]
                };

                if (recordedAnswer["selectedOption"] == null)
                {
                    continue;
                }

                shortQuestion.Answer = new QuestionAnswer
                {
                    AnsweredAt = (DateTime?)recordedAnswer["answeredDt"],
                    Value = (Core.Enums.Answer)(long)recordedAnswer["selectedOption"]
                };

                listOfQuestions.Add(shortQuestion);
            }

            return listOfQuestions.Any() ? listOfQuestions : shortQuestions;
        }
        
        private class SessionIdentifier
        {
            public string Id { get; set; }
        }
    }
}
