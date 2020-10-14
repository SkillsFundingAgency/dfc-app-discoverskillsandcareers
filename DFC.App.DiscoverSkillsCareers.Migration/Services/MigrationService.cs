using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class MigrationService : IMigrationService
    {
        private readonly IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService;

        public MigrationService(IDocumentService<DysacQuestionSetContentModel> dysacQuestionSetDocumentService)
        {
            this.dysacQuestionSetDocumentService = dysacQuestionSetDocumentService;
        }

        public void Start()
        {
            var source = File.ReadAllText("TestData/exampleLegacyData.json");

            dynamic d = JsonConvert.DeserializeObject<dynamic>(source);

            var assessment = new DysacAssessment();
            assessment.AssessmentCode = d["id"];

            assessment.Questions = ConvertToQuestions(d["assessmentState"]["recordedAnswers"]);
            assessment.ShortQuestionResult = ConvertToShortQuestionResult(d["resultData"]);
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
                jobCategoryList.Add(categoryToAdd);
            }

            resultData.JobCategories = jobCategoryList;
        }

        private IEnumerable<JobProfileResult> GetJobProfilesAndSkills(string jobFamilyNameUrl)
        {
            return new List<JobProfileResult>();
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
