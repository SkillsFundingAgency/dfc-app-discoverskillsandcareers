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
        private readonly string destinationDatabaseId;
        private readonly string destinationCollectionId;
        private readonly int cosmosDbDestinationRUs;

        private int saveCount;
        private readonly List<string> erroredSessions = new List<string>();
        
        public MigrationService(
            IDocumentService<DysacTraitContentModel> dysacTraitDocumentService,
            IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService,
            IDocumentClient sourceDocumentClient,
            IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService,
            IDocumentClient destinationDocumentClient,
            string destinationDatabaseId,
            string destinationCollectionId,
            int cosmosDbDestinationRUs)
        {
            this.dysacTraitDocumentService = dysacTraitDocumentService;
            this.dysacFilteringQuestionDocumentService = dysacFilteringQuestionDocumentService;
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
            this.sourceDocumentClient = sourceDocumentClient;
            this.destinationDocumentClient = destinationDocumentClient;
            this.destinationDatabaseId = destinationDatabaseId;
            this.destinationCollectionId = destinationCollectionId;
            this.cosmosDbDestinationRUs = cosmosDbDestinationRUs;
        }

        public async Task Start()
        {
            var startTime = DateTime.Now;
            const int readBatchSize = 1000;
            const string bookmarkPath = "bookmark.txt";
            
            try
            {
                var fetchingSessions = FetchSessionsToMigrate(0, readBatchSize);
                var totalSessionsToMigrateCountTask = GetSessionIdentifiersCount();
                
                await Task.WhenAll(
                    LoadJobCategoriesFromTraits(),
                    LoadFilteringQuestions(),
                    LoadShortQuestionsFromQuestionSet());

                var totalSessionsCount = await totalSessionsToMigrateCountTask;
                var index = 1;
                var outerForeachCount = 0;

                if (File.Exists(bookmarkPath))
                {
                    outerForeachCount = int.Parse(File.ReadAllText(bookmarkPath));
                }

                const int ruCostPerItem = 22;;
                int writeBatchSize = cosmosDbDestinationRUs / ruCostPerItem;

                WriteAndLog($"Writes per second attempting is {writeBatchSize}");

                while (outerForeachCount * readBatchSize < totalSessionsCount)
                {
                    var writingBookmark = File.WriteAllTextAsync(bookmarkPath, outerForeachCount + string.Empty);
                    
                    var sessions = await fetchingSessions;
                    fetchingSessions = FetchSessionsToMigrate(readBatchSize * (outerForeachCount + 1), readBatchSize);

                    var sessionsToWriteSimultaneouslyGroups = sessions.Batch(writeBatchSize);

                    foreach (var sessionsToWriteSimultaneously in sessionsToWriteSimultaneouslyGroups)
                    {
                        var sessionWriteGroupStartTime = DateTime.Now;
                        var creatingDocuments = new List<Task>();

                        foreach (var session in sessionsToWriteSimultaneously)
                        {
                            var sessionId = (string) session["id"];
                            WriteAndLog(
                                $"Started processing assessment {index} of {totalSessionsCount} ({sessionId}) at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");

                            try
                            {
                                var migratedAssessment = new DysacAssessmentForCreate
                                {
                                    id = sessionId
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

                                creatingDocuments.Add(CreateDocument(migratedAssessment, index, totalSessionsCount));
                            }
                            catch (Exception exception)
                            {
                                WriteAndLog(
                                    $"Error processing {index} of {sessions.Count} - {exception.Message} " +
                                    $"at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");

                                erroredSessions.Add(sessionId);
                            }
                            
                            index++;
                        }

                        await Task.WhenAll(creatingDocuments);

                        var duration = DateTime.Now - sessionWriteGroupStartTime;

                        if (1 > duration.TotalSeconds && creatingDocuments.Any())
                        {
                            // Too quick, waiting remaining time

                            var remainder = (int) ((1.0 - duration.TotalSeconds) * 1000);
                            WriteAndLog($"Waiting {remainder}ms at {DateTime.Now:yyyy-MM-dd hh:mm:ss} as used up RUs.");
                            
                            await Task.Delay(remainder);
                        }

                        WriteAndLog(
                            $"Finished creating batch of {writeBatchSize} documents at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
                    }

                    await writingBookmark;
                    outerForeachCount += 1;
                }

                WriteAndLog($"Completed, with {erroredSessions.Count} errors at {DateTime.Now:yyyy-MM-dd hh:mm:ss}.", 1);
            }
            catch (Exception ex)
            {
                WriteAndLog("Fatal error: " + ex.Message + " - " + ex.StackTrace, 1);
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
            File.Delete(bookmarkPath);
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
            WriteAndLog($"Started creating assessment {index} of {count} ({migratedAssessment.id}) - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            
            var start = DateTime.Now;
            double charge;
            
            try
            {
                var resourceResponse = await destinationDocumentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(destinationDatabaseId, destinationCollectionId),
                    migratedAssessment,
                    new RequestOptions());
            
                saveCount += 1;
                charge = resourceResponse.RequestCharge;
            }
            catch (Exception exception)
            {
                WriteAndLog($"Error creating {index} of {count} ({migratedAssessment.id}) - {exception.Message} " +
                    $"at {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
                erroredSessions.Add(migratedAssessment.id);

                return;
            }
            
            WriteAndLog($"Finished creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds. Charge was {charge} RUs");
        }

        private async Task LoadJobCategoriesFromTraits()
        {
            WriteAndLog($"Started fetching all job categories from traits - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var allTraits = await dysacTraitDocumentService
                .GetAsync(document => document.PartitionKey == "Trait");

            allJobCategories = allTraits?
                .SelectMany(trait => trait.JobCategories)
                .GroupBy(jobCategory => jobCategory.Title)
                .Select(jobCategoryGroup => jobCategoryGroup.First())
                .ToList();
            
            WriteAndLog($"Finished fetching all job categories from traits - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
        }
        
        private async Task LoadFilteringQuestions()
        {
            WriteAndLog($"Started fetching filtering questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            filteringQuestions = (await dysacFilteringQuestionDocumentService
                .GetAsync(document => document.PartitionKey == "FilteringQuestion"))!
                .ToList();
            
            WriteAndLog($"Finished fetching filtering questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
        }
        
        private async Task LoadShortQuestionsFromQuestionSet()
        {
            WriteAndLog($"Started fetching short questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
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
            
            WriteAndLog($"Finished fetching short questions - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");            
        }

        private async Task<int> GetSessionIdentifiersCount()
        {
            WriteAndLog($"Started fetching session count - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var query = sourceDocumentClient.CreateDocumentQuery<int>(
                UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                $"select value count(c) from c",
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = -1
                }).AsDocumentQuery();

            var count = (await query.ExecuteNextAsync<int>()).First();

            WriteAndLog($"Finished fetching session count ({count}) - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds");
            
            return count;
        }

        private async Task<List<Dictionary<string, object>>> FetchSessionsToMigrate(int startNumber, int batchSize)
        {
            WriteAndLog($"Started fetching sessions - {startNumber} to {startNumber + batchSize} - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;

            var returnList = new List<Dictionary<string, object>>();
            
            var query = sourceDocumentClient.CreateDocumentQuery<Dictionary<string, object>>(
                UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                $"select c from c order by c._ts asc OFFSET {startNumber} LIMIT {batchSize}",
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

            WriteAndLog($"Finished fetching sessions - {startNumber} to {startNumber + batchSize}. Found {returnList.Count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - " + 
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
    }
}
