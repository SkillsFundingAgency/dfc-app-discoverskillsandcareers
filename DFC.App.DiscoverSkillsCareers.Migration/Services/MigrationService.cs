using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using DFC.App.DiscoverSkillsCareers.Migration.Models;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IDocumentService<DysacTraitContentModel> dysacTraitDocumentService;
        private readonly IDocumentService<DysacAssessment> dysacAssessmentDocumentService;
        private readonly IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService;
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;
        private readonly IDocumentClient documentClient;
        private IEnumerable<JobCategoryContentItemModel> jobCategories = new List<JobCategoryContentItemModel>();
        private IEnumerable<DysacFilteringQuestionContentModel> filteringQuestions = new List<DysacFilteringQuestionContentModel>();
        private IEnumerable<ShortQuestion> shortQuestions = new List<ShortQuestion>();
        private IMapper mapper;
        private readonly MigrationOptions options;

        public MigrationService(IDocumentService<DysacTraitContentModel> dysacTraitDocumentService, IDocumentService<DysacAssessment> dysacAssessmentDocumentService, IDocumentService<DysacFilteringQuestionContentModel> dysacFilteringQuestionDocumentService, IDocumentClient documentClient, IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService, IMapper mapper, MigrationOptions options)
        {
            this.dysacTraitDocumentService = dysacTraitDocumentService;
            this.dysacAssessmentDocumentService = dysacAssessmentDocumentService;
            this.dysacFilteringQuestionDocumentService = dysacFilteringQuestionDocumentService;
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
            this.documentClient = documentClient;
            this.mapper = mapper;
            this.options = options;
        }

        public async Task Start()
        {
            await LoadJobCategoriesAndProfiles();
            await LoadFilteringQuestions();
            shortQuestions = await LoadShortQuestions();
            List<dynamic> itemsToProcess = await LoadLegacyDocuments();

            foreach (var item in itemsToProcess)
            {
                var assessment = new DysacAssessment();
                dynamic d = JsonConvert.DeserializeObject<dynamic>(item.ToString());

                string assessmentCode = d["id"];
                var assessments = await dysacAssessmentDocumentService.GetAsync(x => x.AssessmentCode == assessmentCode);

                if (assessments != null && assessments.Any())
                {
                    assessment = assessments.FirstOrDefault();
                }
                else
                {
                    assessment.Id = Guid.NewGuid();
                }

                assessment.AssessmentCode = assessmentCode;

                assessment.Questions = ConvertToQuestions(d["assessmentState"]["recordedAnswers"]);
                assessment.ShortQuestionResult = ConvertToShortQuestionResult(d["resultData"]);

                if (d["filteredAssessmentState"] != null)
                {
                    assessment.FilteredAssessment = ConvertToFilteredAssessment(d["filteredAssessmentState"], assessment.ShortQuestionResult!);
                }

                var result = await dysacAssessmentDocumentService.UpsertAsync(assessment);
            }
        }

        private async Task<List<dynamic>> LoadLegacyDocuments()
        {
            List<dynamic> itemsToProcess = new List<dynamic>();
            int count = 0;
            string continuation = string.Empty;
            do
            {
                FeedResponse<dynamic> response = await documentClient.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                                                                new FeedOptions
                                                                {
                                                                    MaxItemCount = options.ItemsPerCosmosBatch,
                                                                    RequestContinuation = continuation
                                                                });

                count += response.Count;

                itemsToProcess.AddRange(response);

                continuation = response.ResponseContinuation;
            } while (!string.IsNullOrEmpty(continuation));
            return itemsToProcess;
        }

        private async Task LoadFilteringQuestions()
        {
            var allFilteringQuestions = await dysacFilteringQuestionDocumentService.GetAsync(x => x.PartitionKey == "FilteringQuestion");
            filteringQuestions = allFilteringQuestions.ToList();
        }

        private FilteredAssessment ConvertToFilteredAssessment(dynamic dynamic, ResultData resultData)
        {
            var filteredAssessment = new FilteredAssessment();
            filteredAssessment.JobCategoryAssessments = AddJobCategoryAssessments(dynamic["jobCategories"], resultData.JobCategories.Select(x => new KeyValuePair<string, string>(x.JobFamilyName, x.JobFamilyNameUrl)).ToDictionary(x => x.Key, x => x.Value));
            filteredAssessment.Questions = AddFilterQuestions(filteredAssessment.JobCategoryAssessments);
            AddFilterAnswers(dynamic["recordedAnswers"], filteredAssessment.Questions);

            return filteredAssessment;
        }

        private void AddFilterAnswers(dynamic p, IEnumerable<FilteredAssessmentQuestion> questions)
        {
            foreach (var answer in p)
            {
                var answerTraitAsString = (string)answer["traitCode"].Value;
                var answeredDate = (DateTime)answer["answeredDt"].Value;
                var selectedAnswer = answer["selectedOption"];

                questions.FirstOrDefault(x => x.TraitCode.ToUpperInvariant() == answerTraitAsString.ToUpperInvariant()).Answer = new QuestionAnswer { AnsweredAt = answeredDate, Value = selectedAnswer };
            }
        }

        private IEnumerable<FilteredAssessmentQuestion> AddFilterQuestions(IEnumerable<JobCategoryAssessment> jobCategoryAssessments)
        {
            var groupedQuestions = jobCategoryAssessments.SelectMany(x => x.QuestionSkills).GroupBy(x => x.Key);
            var distinctGroupedQuestions = groupedQuestions.Select(z => z.FirstOrDefault());

            var filteredAssessmentQuestions = new List<FilteredAssessmentQuestion>();

            int questionIndex = 0;
            foreach (var question in distinctGroupedQuestions)
            {
                var filterQuestion = filteringQuestions.FirstOrDefault(x => x.Title.ToUpperInvariant() == question.Key.ToUpperInvariant());

                if (filterQuestion == null)
                {
                    throw new InvalidOperationException($"Filter question {question.Key} not found in Stax Filter Question Repository");
                }

                filteredAssessmentQuestions.Add(new FilteredAssessmentQuestion { Id = filterQuestion.Id, Ordinal = questionIndex, QuestionText = filterQuestion.Text, TraitCode = question.Key });
                questionIndex++;
            }

            return filteredAssessmentQuestions;
        }

        private IEnumerable<JobCategoryAssessment> AddJobCategoryAssessments(dynamic dynamic, Dictionary<string, string> jobCategoryMappings)
        {
            List<JobCategoryAssessment> jobCategoryAssessments = new List<JobCategoryAssessment>();

            foreach (var jobCategory in dynamic)
            {
                var jobCategoryAssessment = new JobCategoryAssessment();
                var questions = jobCategory["questions"];

                foreach (var question in questions)
                {
                    var jobCategoryName = (string)jobCategory["jobCategoryName"];
                    jobCategoryAssessment.JobCategory = jobCategoryMappings[jobCategoryName];

                    string skill = question["Skill"].Value;
                    var skillQuestion = filteringQuestions.FirstOrDefault(x => x.Title.ToUpperInvariant() == skill.ToUpperInvariant());

                    if (skillQuestion == null)
                    {
                        throw new InvalidOperationException($"Filter question {skill} not found in Stax Filter Question Repository");
                    }

                    jobCategoryAssessment.QuestionSkills.Add(skill, skillQuestion.Skills.FirstOrDefault().Ordinal.Value);

                    jobCategoryAssessments.Add(jobCategoryAssessment);
                }
            }

            return jobCategoryAssessments;
        }

        private async Task LoadJobCategoriesAndProfiles()
        {
            var allTraits = await dysacTraitDocumentService.GetAsync(x => x.PartitionKey == "Trait");

            var allJobCategories = allTraits.SelectMany(x => x.JobCategories).GroupBy(z => z.WebsiteURI).Select(y => y.FirstOrDefault());

            jobCategories = allJobCategories;
        }

        private ResultData ConvertToShortQuestionResult(dynamic dynamic)
        {
            //Result data object has changed slightly...
            var resultData = new ResultData();

            BuildTraits(dynamic, resultData);
            BuildJobCategories(dynamic, resultData);

            if (resultData.JobCategories != null || resultData.JobProfiles != null)
            {
                return resultData;
            }

            return null;
        }

        private void BuildJobCategories(dynamic dynamic, ResultData resultData)
        {
            if (dynamic != null)
            {
                var jobCategoriesDynamic = dynamic["jobFamilies"];
                var jobCategoryList = new List<JobCategoryResult>();

                foreach (var jobCategory in jobCategoriesDynamic)
                {
                    //Get the job profile and the skills
                    JobCategoryResult categoryToAdd = JsonConvert.DeserializeObject<JobCategoryResult>(JsonConvert.SerializeObject(jobCategory));
                    categoryToAdd.JobProfiles = GetJobProfilesAndSkills(categoryToAdd.JobFamilyNameUrl);
                    categoryToAdd.SkillQuestions = categoryToAdd.JobProfiles.SelectMany(x => x.SkillCodes).GroupBy(z => z).Select(y => y.FirstOrDefault());
                    categoryToAdd.Total = categoryToAdd.SkillQuestions.Count();
                    jobCategoryList.Add(categoryToAdd);
                }

                resultData.JobCategories = jobCategoryList;
            }
        }

        private IEnumerable<JobProfileResult> GetJobProfilesAndSkills(string jobFamilyNameUrl)
        {
            var resultsToReturn = new List<JobProfileResult>();

            var category = jobCategories.FirstOrDefault(x => x.WebsiteURI.Contains(jobFamilyNameUrl));

            //Old DYSAC, "Computing, Technology and Digital" is one category, needs sorting.
            if (category != null)
            {
                foreach (var profile in category.JobProfiles)
                {
                    resultsToReturn.Add(new JobProfileResult { Title = profile.Title, SkillCodes = profile.Skills.Select(x => x.Title).ToList() });
                }
            }

            return resultsToReturn;
        }

        private static void BuildTraits(dynamic dynamic, ResultData resultData)
        {
            if (dynamic != null)
            {
                var traitsDynamic = dynamic["traits"];

                var traits = new List<TraitResult>();

                foreach (var item in traitsDynamic)
                {
                    traits.Add(new TraitResult { Text = item["traitText"], TotalScore = item["totalScore"], TraitCode = item["traitCode"] });
                }

                resultData.Traits = traits;
            }
        }

        private IEnumerable<ShortQuestion> ConvertToQuestions(dynamic dynamic)
        {
            var listOfQuestions = new List<ShortQuestion>();

            foreach (var question in dynamic)
            {
                var shortQuestion = new ShortQuestion();
                shortQuestion.Id = Guid.NewGuid();
                shortQuestion.QuestionText = question["questionText"];
                shortQuestion.Ordinal = question["questionNumber"] - 1;
                shortQuestion.IsNegative = question["isNegative"];
                shortQuestion.Trait = question["traitCode"];

                if (question["selectedOption"] != null)
                {
                    shortQuestion.Answer = new QuestionAnswer { AnsweredAt = question["answeredDt"], Value = (Core.Enums.Answer)question["selectedOption"] };
                }

                listOfQuestions.Add(shortQuestion);
            }

            if (!listOfQuestions.Any())
            {
                return shortQuestions;
            }

            return listOfQuestions;
        }

        private async Task<IEnumerable<ShortQuestion>> LoadShortQuestions()
        {
            var questionSets = await dysacQuestionSetDocumentService.GetAsync(x => x.PartitionKey == "QuestionSet");

            var shortQuestions = questionSets.FirstOrDefault().ShortQuestions.Select(x => mapper.Map<ShortQuestion>(x));

            return shortQuestions;
        }
    }
}
