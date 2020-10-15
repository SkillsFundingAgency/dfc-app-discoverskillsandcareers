using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IDocumentService<DysacTraitContentModel> dysacTraitDocumentService;
        private readonly IDocumentService<DysacAssessment> dysacAssessmentDocumentService;
        private IEnumerable<JobCategoryContentItemModel> jobCategories = new List<JobCategoryContentItemModel>();

        public MigrationService(IDocumentService<DysacTraitContentModel> dysacQuestionSetDocumentService, IDocumentService<DysacAssessment> dysacAssessmentDocumentService)
        {
            this.dysacTraitDocumentService = dysacQuestionSetDocumentService;
            this.dysacAssessmentDocumentService = dysacAssessmentDocumentService;
        }

        public async Task Start()
        {
            var source = File.ReadAllText("TestData/exampleLegacyData.json");

            await LoadJobCategoriesAndProfiles();

            dynamic d = JsonConvert.DeserializeObject<dynamic>(source);

            var assessment = new DysacAssessment();

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

            await dysacAssessmentDocumentService.UpsertAsync(assessment);
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

            return resultData;
        }

        private void BuildJobCategories(dynamic dynamic, ResultData resultData)
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

        private IEnumerable<JobProfileResult> GetJobProfilesAndSkills(string jobFamilyNameUrl)
        {
            var resultsToReturn = new List<JobProfileResult>();

            var category = jobCategories.FirstOrDefault(x => x.WebsiteURI.Contains(jobFamilyNameUrl));

            foreach (var profile in category.JobProfiles)
            {
                resultsToReturn.Add(new JobProfileResult { Title = profile.Title, SkillCodes = profile.Skills.Select(x => x.Title).ToList() });
            }

            return resultsToReturn;
        }

        private static void BuildTraits(dynamic dynamic, ResultData resultData)
        {
            var traitsDynamic = dynamic["traits"];

            var traits = new List<TraitResult>();

            foreach (var item in traitsDynamic)
            {
                traits.Add(new TraitResult { Text = item["traitText"], TotalScore = item["totalScore"], TraitCode = item["traitCode"] });
            }

            resultData.Traits = traits;
        }

        private IEnumerable<ShortQuestion> ConvertToQuestions(dynamic dynamic)
        {
            var listOfQuestions = new List<ShortQuestion>();

            foreach (var question in dynamic)
            {
                var shortQuestion = new ShortQuestion();
                shortQuestion.Id = Guid.NewGuid(); //Swap Out with actual question...?
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

            return listOfQuestions;
        }
    }
}
