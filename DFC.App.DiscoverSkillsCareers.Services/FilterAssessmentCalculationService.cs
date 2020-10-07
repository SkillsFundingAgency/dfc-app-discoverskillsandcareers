//using DFC.App.DiscoverSkillsCareers.Core.Enums;
//using DFC.App.DiscoverSkillsCareers.Models;
//using DFC.App.DiscoverSkillsCareers.Models.Assessment;
//using DFC.App.DiscoverSkillsCareers.Services.Contracts;
//using DFC.Compui.Cosmos.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DFC.App.DiscoverSkillsCareers.Services
//{
//    public class FilterAssessmentCalculationService : IFilterAssessmentCalculationService
//    {
//        private readonly IDocumentService<DysacFilteringQuestionContentModel> _filteringQuestionDocumentService;
//        private readonly IJobCategoryRepository _jobCategoryRepository;

//        public FilterAssessmentCalculationService(IDocumentService<DysacFilteringQuestionContentModel> filteringQuestionDocumentService, IJobCategoryRepository jobCategoryRepository)
//        {
//            _filteringQuestionDocumentService = _ilteringQuestionDocumentService;
//            _jobCategoryRepository = jobCategoryRepository;
//        }

//        public async Task ProcessAssessment(DysacAssessment assessment)
//        {
//            var answeredTraits = assessment.FilteredAssessment.Answers.Select(z => z.TraitCode);

//            foreach (var jobCategoryState in assessment.FilteredAssessment.JobCategoryAssessments.Where(j => j.AssessmentFilteredQuestions.All(y => answeredTraits.Contains(y.TraitCode))))
//            {
//                var questions = await _filteringQuestionDocumentService.GetAsync(x=>x.PartitionKey == .GetQuestions(jobCategoryState.QuestionSetVersion);
//                var category = await _jobCategoryRepository.GetJobCategory(jobCategoryState.JobCategoryCode);

//                var jobProfiles =
//                    jobCategoryState.Skills.SelectMany((s, i) =>
//                    {
//                        var c = category.Skills.First(cs => cs.ONetAttribute.EqualsIgnoreCase(s.Skill));
//                        return c.JobProfiles.Select(p => new
//                        {
//                            QuestionNumber = s.QuestionNumber,
//                            Profile = p.JobProfile,
//                            Answer = p.Included ? AnswerOption.Yes : AnswerOption.No
//                        });
//                    })
//                        .GroupBy(p => p.Profile)
//                        .Select(g => new { Profile = g.Key, Answers = g.OrderBy(q => q.QuestionNumber).Select(q => q.Answer) });


//                var categoryAnswers =
//                    assessment.FilteredAssessmentState.GetAnswersForCategory(jobCategoryState.JobCategoryCode);

//                var answers =
//                        categoryAnswers
//                            .OrderBy(a => a.Index)
//                            .Select(a => a.Answer.SelectedOption)
//                            .ToArray();

//                var suggestedProfiles = new List<string>();

//                foreach (var jobProfile in jobProfiles)
//                {
//                    if (jobProfile.Answers.SequenceEqual(answers, EqualityComparer<AnswerOption>.Default))
//                    {
//                        suggestedProfiles.Add(jobProfile.Profile);
//                    }
//                }

//                var jobCategoryResult =
//                    assessment.ResultData.JobCategories.Single(jf => jf.JobCategoryCode.EqualsIgnoreCase(jobCategoryState.JobCategoryCode));

//                var recordedAnswers = categoryAnswers.Select(a => a.Answer).ToArray();

//                jobCategoryResult.FilterAssessmentResult = new FilterAssessmentResult
//                {
//                    JobFamilyName = jobCategoryState.JobCategoryName,
//                    CreatedDt = DateTime.UtcNow,
//                    QuestionSetVersion = assessment.CurrentQuestionSetVersion,
//                    MaxQuestions = assessment.MaxQuestions,
//                    SuggestedJobProfiles = suggestedProfiles,
//                    RecordedAnswerCount = recordedAnswers.Length,
//                    RecordedAnswers = recordedAnswers,
//                    WhatYouToldUs = ComputeWhatYouToldUs(recordedAnswers, questions).Distinct().ToArray()
//                };
//            }


//            assessment.UpdateJobCategoryQuestionCount();
//        }

//        public List<string> ComputeWhatYouToldUs(Answer[] categoryAnswers, Question[] questions)
//        {
//            var whatYouToldUs = new List<string>();

//            foreach (var answer in categoryAnswers)
//            {
//                String text = null;
//                if (answer.SelectedOption == AnswerOption.No)
//                {
//                    text = questions
//                        .FirstOrDefault(q => q.QuestionId.EqualsIgnoreCase(answer.QuestionId))?.NegativeResultDisplayText;
//                }
//                else
//                {
//                    text = questions
//                        .FirstOrDefault(q => q.QuestionId.EqualsIgnoreCase(answer.QuestionId))
//                        ?.PositiveResultDisplayText;
//                }

//                if (!String.IsNullOrWhiteSpace(text))
//                {
//                    whatYouToldUs.Add(text);
//                }
//            }

//            return whatYouToldUs;
//        }
//    }
//}
