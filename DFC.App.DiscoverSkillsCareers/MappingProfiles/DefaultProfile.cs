using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac.PersonalityTrait;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    [ExcludeFromCodeCoverage]
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<AnswerDetail, AnswerDetailsViewModel>()
                .ForMember(d => d.AnsweredOn, x => x.MapFrom(s => s.AnsweredDt))
                .ForMember(d => d.Answer, x => x.MapFrom(s => s.SelectedOption))
                ;

            CreateMap<FilterAssessmentResult, FilterAssessmentResultViewModel>();

            CreateMap<GetResultsResponse, ResultsIndexResponseViewModel>();

            CreateMap<GetQuestionResponse, QuestionGetResponseViewModel>()
                .ForMember(d => d.Answer, s => s.MapFrom(a => a.RecordedAnswer!.Value))
                .ForMember(d => d.Started, s => s.MapFrom(a => a.StartedDt))
                .ForMember(d => d.PercentageComplete, s => s.MapFrom(a => a.PercentComplete))
                .ForMember(d => d.AvailableQuestionsCount, s => s.MapFrom(a => a.MaxQuestionsCount))
                ;

            CreateMap<GetQuestionResponse, FilterQuestionIndexResponseViewModel>();

            CreateMap<JobCategoryResult, JobCategoryResultViewModel>();

            CreateMap<JobProfileResult, JobProfileResultViewModel>()
                .ForMember(d => d.JobCategoryName, s => s.MapFrom(a => a.JobCategory));

            CreateMap<TraitValue, TraitValueViewModel>();

            CreateMap<PersonalityQuestionSet, DysacQuestionSetContentModel>().ForMember(
                d => d.ShortQuestions,
                s => s.MapFrom(z => Construct(z.Questions.ContentItems.ToList())));
        }

        private static IEnumerable<DysacShortQuestionContentItemModel> Construct(List<QuestionContentItem> items)
        {
            var listOfQuestions = new List<DysacShortQuestionContentItemModel>();
            string prefixShortQuestion = "<<contentapiprefix>>/personalityshortquestion/";
            string prefixTrait = "<<contentapiprefix>>/personalitytrait/";

            foreach (var item in items)
            {
                var question = new DysacShortQuestionContentItemModel
                {
                    Traits = new List<DysacTraitContentItemModel>(),
                    Title = item.DisplayText,
                    Ordinal = item.Ordinal,
                    Impact = item.Impact,
                    ItemId = new Guid(item.GraphSync.NodeId.Replace(prefixShortQuestion, string.Empty)),
                    LastCached = DateTime.UtcNow,
                };

                question.Traits.AddRange(
                    item.Trait.ContentItems.Select(z =>
                    new DysacTraitContentItemModel
                    {
                        Ordinal = item.Ordinal,
                        Description = z.DisplayText,
                        Title = z.DisplayText,
                        JobCategories = ConstructJobCategories(z.JobProfileCategories),
                        ItemId = new Guid(z.GraphSync.NodeId.Replace(prefixTrait, string.Empty)),
                        LastCached = DateTime.UtcNow,
                    }));
                listOfQuestions.Add(question);
            }

            return listOfQuestions;
        }

        private static List<JobCategoryContentItemModel> ConstructJobCategories(JobProfileCategories jobProfileCategories)
        {
            var listToReturn = new List<JobCategoryContentItemModel>();

            listToReturn.AddRange(
                jobProfileCategories.ContentItems.Select(
                    x => new JobCategoryContentItemModel
                    {
                        Description = x.DisplayText,
                        LastCached = DateTime.UtcNow,
                    }));

            return listToReturn;
        }
    }
}
