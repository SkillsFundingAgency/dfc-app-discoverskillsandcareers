﻿using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using DFC.App.DiscoverSkillsCareers.Models.Result;
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
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Linq;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IDocumentStore documentStore;
        private readonly IDocumentClient sourceDocumentClient;
        private readonly Container destinationDocumentContainer;
        
        private List<JobCategoryContentItemModel> allJobCategories = new List<JobCategoryContentItemModel>();
        private List<JobProfileContentItemModel> allJobProfiles = new List<JobProfileContentItemModel>();
        private List<DysacFilteringQuestionContentModel> filteringQuestions = new List<DysacFilteringQuestionContentModel>();
        private List<ShortQuestion> shortQuestions = new List<ShortQuestion>();
        private readonly StringBuilder logger = new StringBuilder();
        private readonly int cosmosDbDestinationRUs;
        private readonly bool useBookmark;
        private readonly DateTime? cutoffDateTime;

        private int saveCount;
        private List<string> erroredSessions = new List<string>();
        
        public MigrationService(
            IDocumentStore documentStore,
            IDocumentClient sourceDocumentClient,
            CosmosClient destinationDocumentClient,
            string destinationDatabaseId,
            string destinationCollectionId,
            int cosmosDbDestinationRUs,
            bool useBookmark,
            DateTime? cutoffDateTime)
        {
            this.documentStore = documentStore;
            this.sourceDocumentClient = sourceDocumentClient;
            this.destinationDocumentContainer =
                destinationDocumentClient.GetContainer(destinationDatabaseId, destinationCollectionId);
            this.cosmosDbDestinationRUs = cosmosDbDestinationRUs;
            this.useBookmark = useBookmark;
            this.cutoffDateTime = cutoffDateTime;
        }

        public async Task Start()
        {
            var startTime = DateTime.Now;
            const int readBatchSize = 300;
            const string bookmarkPath = "bookmark.txt";
            var allOkay = true;
            
            try
            {
                var fetchingSessionsIds = GetSessionIdentifiers();
                
                await Task.WhenAll(
                    LoadJobCategoriesFromTraits(),
                    LoadFilteringQuestions(),
                    LoadShortQuestionsFromQuestionSet());

                var sessionsIds = await fetchingSessionsIds;
                var outerForeachCount = 0;

                if (useBookmark && File.Exists(bookmarkPath))
                {
                    outerForeachCount = int.Parse(File.ReadAllText(bookmarkPath));
                    WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Using bookmark - starting at {outerForeachCount * readBatchSize}");
                }
                else
                {
                    WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Not using bookmark or cannot find bookmark");
                }
                
                var fetchingSessions = FetchSessionsToMigrate(
                    sessionsIds.Skip(readBatchSize * (outerForeachCount + 1)).Take(readBatchSize).ToList());
                
                var totalSessionsCount = sessionsIds.Count;
                var index = readBatchSize * (outerForeachCount + 1) + 1;

                const int ruCostPerItem = 185;
                int writeBatchSize = (int)Math.Ceiling(cosmosDbDestinationRUs / (ruCostPerItem * 1.3));

                WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Writes per second attempting is {writeBatchSize}");

                while (outerForeachCount * readBatchSize < totalSessionsCount)
                {
                    var writingBookmark = File.WriteAllTextAsync(bookmarkPath, outerForeachCount + string.Empty);
                    
                    var sessions = await fetchingSessions;
                    fetchingSessions = FetchSessionsToMigrate(
                        sessionsIds.Skip(readBatchSize * (outerForeachCount + 1)).Take(readBatchSize).ToList());

                    var sessionsToWriteSimultaneouslyGroups = sessions.Batch(writeBatchSize);

                    foreach (var sessionsToWriteSimultaneously in sessionsToWriteSimultaneouslyGroups)
                    {
                        var sessionWriteGroupStartTime = DateTime.Now;
                        var creatingDocuments = new List<Task>();

                        foreach (var session in sessionsToWriteSimultaneously)
                        {
                            ProcessItem(session, index++, creatingDocuments, totalSessionsCount, sessions.Count);
                        }

                        await Task.WhenAll(creatingDocuments);

                        var duration = DateTime.Now - sessionWriteGroupStartTime;

                        if (1.0 > duration.TotalSeconds && creatingDocuments.Any())
                        {
                            // Too quick, waiting remaining time

                            var remainder = (int) ((1.0 - duration.TotalSeconds) * 1000);
                            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Waiting {remainder}ms as used up RU/s.");
                            
                            await Task.Delay(remainder);
                        }

                        WriteAndLog(
                            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Finished creating batch of {writeBatchSize} documents");
                    }

                    await writingBookmark;
                    outerForeachCount += 1;
                }

                WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Completed, with {erroredSessions.Count} errors.", 1);
            }
            catch (Exception ex)
            {
                WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Fatal error: " + ex.Message + " - " + ex.StackTrace, 1);
                allOkay = false;
            }

            var originalErroredSessions = erroredSessions;
            erroredSessions = new List<string>();

            foreach (var errorSession in originalErroredSessions)
            {
                var sessionId = errorSession.Split('|')[0];
                var partitionKey = errorSession.Split('|')[1];
                
                var list = new List<(string id, string partitionKey)>
                {
                    (sessionId, partitionKey)
                };

                var sessionsToMigrate = await FetchSessionsToMigrate(list);
                var creatingDocuments = new List<Task>();
                
                ProcessItem(sessionsToMigrate.First(), -1, creatingDocuments, -1, -1);
                await Task.WhenAll(creatingDocuments);
            }

            var footerReport = new StringBuilder();
            footerReport.AppendLine("Results:");
            footerReport.Append($"{saveCount} items moved.\r\n\r\n");
            footerReport.AppendLine("The following session IDs had errors while moving:");
            
            foreach (var error in erroredSessions)
            {
                footerReport.AppendLine($"{error}");                                
            }

            var totalDuration = (DateTime.Now - startTime).ToString();

            footerReport.AppendLine();
            footerReport.AppendLine($"Finished. Time taken {totalDuration}");

            WriteAndLog(footerReport.ToString());

            File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}-report.txt", logger.ToString());

            if (allOkay)
            {
                File.Delete(bookmarkPath);
            }
        }

        private void ProcessItem(Dictionary<string, object> session, int index, List<Task> creatingDocuments, int totalSessionsCount, int sessionsCount)
        {
            var sessionId = (string) session["id"];
            var partitionKey = ((DateTime) session["lastUpdatedDt"]).ToUniversalTime().ToString("yyyy-MM-dd");

            try
            {
                var migratedAssessment = new DysacAssessmentForCreate
                {
                    id = sessionId,
                    PartitionKey = partitionKey
                };

                var recordedAnswers = ((session["assessmentState"] as JObject)!
                        .ToObject<Dictionary<string, object>>()!
                        ["recordedAnswers"] as JArray)!
                    .Select(recordedAnswersObj => recordedAnswersObj.ToObject<Dictionary<string, object>>())
                    .ToList();

                migratedAssessment.Questions = ConvertToQuestions(recordedAnswers);

                migratedAssessment.ShortQuestionResult = ConvertToShortQuestionResult(
                    (session["resultData"] as JObject)?.ToObject<Dictionary<string, object>>());

                migratedAssessment.FilteredAssessment = ConvertToFilteredAssessment(
                    (session["filteredAssessmentState"] as JObject)
                    ?.ToObject<Dictionary<string, object>>(),
                    migratedAssessment.ShortQuestionResult!);

                creatingDocuments.Add(CreateDocument(migratedAssessment, index, totalSessionsCount));
            }
            catch (Exception exception)
            {
                WriteAndLog(
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Error processing {index} of {sessionsCount} - {exception.Message}");

                erroredSessions.Add($"{sessionId}|{partitionKey}|{exception.Message}");
            }
        }
        
        private void WriteAndLog(string message, int blankLinesAfter = 0)
        {
            Console.WriteLine(message);
            logger.AppendLine(message);

            for (var idx = 0; idx < blankLinesAfter; idx++)
            {
                Console.WriteLine(string.Empty);
                logger.AppendLine(string.Empty);
            }
        }

        private async Task CreateDocument(DysacAssessmentForCreate migratedAssessment, int index, int count)
        {
            var start = DateTime.Now;
            double charge;

            try
            {
                var requestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = false };
                
                var resourceResponse = await destinationDocumentContainer.UpsertItemAsync(
                    migratedAssessment,
                    new PartitionKey(migratedAssessment.PartitionKey),
                    requestOptions);

                saveCount += 1;
                charge = resourceResponse.RequestCharge;
            }
            catch (Exception exception)
            {
                WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error creating {index} of {count} ({migratedAssessment.id}) - {exception.Message}");
                erroredSessions.Add($"{migratedAssessment.id}|{migratedAssessment.PartitionKey}|{exception.Message}");
                
                return;
            }
            
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Finished creating assessment {index} of {count} ({migratedAssessment.id}) - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds. Charge was {charge} RUs");
        }

        private async Task LoadJobCategoriesFromTraits()
        {
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Started fetching all job categories from traits");
            var start = DateTime.Now;
            
            var allTraits = await documentStore
                .GetAllContentAsync<DysacTraitContentModel>("Trait");

            allJobCategories = allTraits
                .SelectMany(trait => trait.JobCategories)
                .GroupBy(jobCategory => jobCategory.Title)
                .Select(jobCategoryGroup => jobCategoryGroup.First())
                .ToList();
            
            var fullCategories =
                await documentStore
                    .GetAllContentAsync<DysacJobProfileCategoryContentModel>("JobProfileCategory");

            allJobProfiles = fullCategories!
                .SelectMany(jobCategory => jobCategory.JobProfiles)
                .GroupBy(jobProfile => jobProfile.Title)
                .Select(jobProfileGroup => jobProfileGroup.First())
                .ToList();
            
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Finished fetching all job categories from traits - {allJobCategories.Count} found - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
        }
        
        private async Task LoadFilteringQuestions()
        {
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Started fetching filtering questions");
            var start = DateTime.Now;
            
            filteringQuestions = (await documentStore
                .GetAllContentAsync<DysacFilteringQuestionContentModel>("FilteringQuestion"))!
                .ToList();
            
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Finished fetching filtering questions - {filteringQuestions.Count} found - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
        }
        
        private async Task LoadShortQuestionsFromQuestionSet()
        {
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Started fetching short questions");
            var start = DateTime.Now;
            
            var questionSets = await documentStore
                .GetAllContentAsync<DysacQuestionSetContentModel>("QuestionSet");

            shortQuestions = questionSets
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
            
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Finished fetching short questions - {shortQuestions!.Count} found - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");            
        }

        private async Task<List<(string id, string partitionKey)>> GetSessionIdentifiers()
        {
            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Started fetching session identifiers");
            var start = DateTime.Now;
            var cutoffDateTimeString = cutoffDateTime?.ToString("u");

            var returnList = new List<(string id, string partitionKey)>();
            
            var query = sourceDocumentClient.CreateDocumentQuery<int>(
                UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                cutoffDateTime != null ?
                    $"select c.id, c.partitionKey from c where c.startedDt > '{cutoffDateTimeString}' order by c._ts asc"
                    : "select c.id, c.partitionKey from c order by c._ts asc",
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = 20000
                }).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                returnList.AddRange((await query.ExecuteNextAsync<Dictionary<string, object>>())
                    .Select(dyn => (dyn["id"] as string, dyn["partitionKey"] as string))
                    .ToList());
                
                WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Fetched {returnList.Count} session identifiers so far - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            }

            WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Finished fetching session identifiers. Found {returnList.Count} - " + 
                $"took {(DateTime.Now - start).TotalSeconds} seconds");

            return returnList;
        }

        private async Task<List<Dictionary<string, object>>> FetchSessionsToMigrate(
            List<(string id, string partitionKey)> sessionIds)
        {
            var start = DateTime.Now;
            var cutoffDateTimeString = cutoffDateTime?.ToString("u");
            
            var returnList = new List<Dictionary<string, object>>();
            var sql = cutoffDateTime != null
                ? $"select c from c where c.startedDt > '{cutoffDateTimeString}'" +
                  " and ("
                  + string.Join(" OR ",
                      sessionIds.Select(session =>
                          $"(c.id='{session.id}' and c.partitionKey='{session.partitionKey}')"))
                  + ")"
                : "select c from c where "
                  + string.Join(" OR ",
                      sessionIds.Select(session =>
                          $"(c.id='{session.id}' and c.partitionKey='{session.partitionKey}')"));

            var query = sourceDocumentClient.CreateDocumentQuery<Dictionary<string, object>>(
                UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                sql,
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = -1
                }).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                returnList.AddRange((await query.ExecuteNextAsync<Dictionary<string, object>>())
                    .Select(dyn => (dyn["c"] as JObject)!.ToObject<Dictionary<string, object>>())
                    .ToList());
            }

            WriteAndLog($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} - Finished fetching sessions. Found {returnList.Count} - " + 
                $"took {(DateTime.Now - start).TotalSeconds} seconds");

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

        private void AddFilterAnswers(List<Dictionary<string, object>> recordedAnswers, List<FilteredAssessmentQuestion> questions)
        {
            foreach (var recordedAnswer in recordedAnswers)
            {
                var answerTraitAsString = (string)recordedAnswer["traitCode"];
                var answeredDate = (DateTime)recordedAnswer["answeredDt"];
                var selectedAnswer = (Answer)(long)recordedAnswer["selectedOption"];

                var questionToUpdate = questions
                    .FirstOrDefault(question => string.Equals(question.TraitCode!, answerTraitAsString, StringComparison.InvariantCultureIgnoreCase))!;

                if (questionToUpdate == null)
                {
                    questionToUpdate = new FilteredAssessmentQuestion
                    {
                        QuestionText = (string)recordedAnswer["questionText"],
                        Id = Guid.NewGuid(),
                        TraitCode = answerTraitAsString,
                    };
                    
                    questions.Add(questionToUpdate);
                    WriteAndLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Filter question '{questionToUpdate.QuestionText}' was missing from job category gleamed questions - adding");
                }
                
                questionToUpdate.Answer = new QuestionAnswer { AnsweredAt = answeredDate, Value = selectedAnswer };
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
                    throw new InvalidOperationException(
                        $"Filter question {question.Key} not found in Stax Filter Question Repository");
                }

                filteredAssessmentQuestions.Add(new FilteredAssessmentQuestion
                {
                    Id = filterQuestion?.Id ?? Guid.NewGuid(),
                    Ordinal = questionIndex++,
                    QuestionText = filterQuestion?.Text ?? "Not found",
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

                    if (!jobCategoryMappings.ContainsKey(jobCategoryName))
                    {
                        continue;
                    }
                    
                    jobCategoryAssessment.JobCategory = jobCategoryMappings[jobCategoryName];

                    var skill = (string)question["Skill"];
                    var skillQuestion = filteringQuestions
                        .FirstOrDefault(filteringQuestion =>
                            string.Equals(filteringQuestion.Title!, skill, StringComparison.InvariantCultureIgnoreCase));

                    if (skillQuestion == null)
                    {
                        throw new InvalidOperationException($"Filter question {skill} not found in Stax Filter Question Repository");
                    }

                    jobCategoryAssessment.QuestionSkills.Add(skill, skillQuestion?.Skills.First().Ordinal ?? int.MaxValue);
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

            if (returnItem.JobCategories != null)
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
                var fullProfile = allJobProfiles.First(x => x.Title == profile.Title);
                
                resultsToReturn.Add(new JobProfileResult
                {
                    Title = profile.Title,
                    SkillCodes = fullProfile.Skills.Select(skill => skill.Title).ToList()
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
                    Value = (Answer)(long)recordedAnswer["selectedOption"]
                };

                listOfQuestions.Add(shortQuestion);
            }

            return listOfQuestions.Any() ? listOfQuestions : shortQuestions;
        }
    }
}
