﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Core.Helpers;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.Dysac.PersonalityTrait;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    [ExcludeFromCodeCoverage]
    public class DefaultProfile : Profile
    {
        private static string prefixShortQuestion = "<<contentapiprefix>>/personalityshortquestion/";
        private static string prefixTrait = "<<contentapiprefix>>/personalitytrait/";
        private static string skillIdPrefix = "<<contentapiprefix>>/socskillsmatrix/";
        private static string filteringIdPrefix = "<<contentapiprefix>>/personalityfilteringquestion/";
        private static string jobProfilePrefix = "<<contentapiprefix>>/jobprofile/";
        private static string jobCategoryPrefix = "<<contentapiprefix>>/jobprofilecategory/";

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

            CreateMap<PersonalityFilteringQuestion, DysacFilteringQuestionContentModel>()
              .ForMember(d => d.Id, s => s.MapFrom(a => a.GraphSync.NodeId.Replace(filteringIdPrefix, string.Empty)))
              .ForMember(d => d.Text, s => s.MapFrom(a => a.Text))
              .ForMember(d => d.Ordinal, s => s.MapFrom(a => a.Ordinal))
              .ForMember(d => d.Skills, s => s.MapFrom(a => ConstructSkills(a.SOCSkillsMatrix.ContentItems)));

            CreateMap<PersonalityTrait, DysacTraitContentModel>()
                .ForMember(d => d.ImagePath, s => s.MapFrom(z => z.ImagePath))
                .ForMember(d => d.Description, s => s.MapFrom(z => z.Description))
                .ForMember(d => d.Id, s => s.MapFrom(a => new Guid(a.GraphSync.NodeId.Replace(prefixTrait, string.Empty))))
                .ForMember(d => d.JobCategories, s => s.MapFrom(z => ConstructJobCategories(z.JobProfileCategories)))
                .ForMember(d => d.Title, s => s.MapFrom(z => z.DisplayText.ToUpperInvariant()));
        }

        private static List<DysacSkillContentItemModel> ConstructSkills(SOCSkillsMatrixContentItem[] contentItems)
        {
            var listToReturn = contentItems.Select(x => new DysacSkillContentItemModel
            {
                ONetRank = !string.IsNullOrEmpty(x.ONetRank) ?
                    decimal.Parse(x.ONetRank) : (decimal?)null,
                Ordinal = x.Ordinal,
                ItemId = new Guid(x.GraphSync.NodeId.Replace(skillIdPrefix, string.Empty)),
                Title = GeneralHelper.GetGenericSkillName(x.RelatedSkill),
                AttributeType = x.ONetAttributeType,
                LastCached = DateTime.UtcNow,
            }).ToList();

            return listToReturn;
        }

        private static IEnumerable<DysacShortQuestionContentItemModel> Construct(List<QuestionContentItem> items)
        {
            var listOfQuestions = new List<DysacShortQuestionContentItemModel>();

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
                        ItemId = new Guid(x.GraphSync.NodeId.Replace(jobCategoryPrefix, string.Empty)),
                        Description = x.DisplayText,
                        Title = x.DisplayText,
                        JobProfiles = ConstructJobProfiles(x.JobProfiles),
                        LastCached = DateTime.UtcNow,
                    }));

            return listToReturn;
        }

        private static List<JobProfileContentItemModel> ConstructJobProfiles(List<JobProfile> jobProfiles)
        {
            var listToReturn = new List<JobProfileContentItemModel>();
            if (jobProfiles != null)
            {
                listToReturn.AddRange(jobProfiles.Select(x =>
                    new JobProfileContentItemModel
                    {
                        ItemId = new Guid(x.GraphSync.NodeId.Replace(jobProfilePrefix, string.Empty)),
                        Title = x.DisplayText,
                        JobProfileWebsiteUrl = x.PageLocation.FullUrl,
                        LastCached = DateTime.UtcNow,
                    }));
            }

            return listToReturn;
        }
    }
}
