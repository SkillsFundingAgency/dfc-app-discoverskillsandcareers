﻿using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    [ExcludeFromCodeCoverage]
    public class DysacProfile : Profile
    {
        public DysacProfile()
        {
            CreateMap<CustomLinkDetails, DysacShortQuestionContentItemModel>();
            CreateMap<CustomLinkDetails, ApiShortQuestion>();
            CreateMap<CustomLinkDetails, ApiTrait>();
            CreateMap<CustomLinkDetails, ApiSkill>();
            CreateMap<CustomLinkDetails, ApiJobCategory>();
            CreateMap<CustomLinkDetails, ApiJobProfile>();
            CreateMap<CustomLinkDetails, ApiONetOccupationalCode>();

            CreateMap<ApiQuestionSet, DysacQuestionSetContentModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.Type, s => s.MapFrom(a => a.Type.ToLower()))
                .ForMember(d => d.ShortQuestions, s => s.MapFrom(z => Construct(z.ContentItems)));

            CreateMap<ApiTrait, DysacTraitContentModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.JobCategories, s => s.MapFrom(z => ConstructJobCategories(z.ContentItems)))
                .ForMember(d => d.Title, s => s.MapFrom(z => z.Title.ToUpperInvariant()));

            CreateMap<ApiSkill, DysacSkillContentModel>()
              .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId));

            CreateMap<ApiJobProfile, JobProfileContentItemModel>();

            CreateMap<JobProfileContentItemModel, JobProfileResult>()
                .ForMember(d => d.Title, s => s.MapFrom(a => a.Title))
                .ForMember(d => d.SkillCodes, s => s.MapFrom(a => a.Skills.Select(z => z.Title)));

            CreateMap<DysacShortQuestionContentItemModel, ShortQuestion>()
             .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
             .ForMember(d => d.QuestionText, s => s.MapFrom(a => a.Title))
             .ForMember(d => d.IsNegative, s => s.MapFrom(a => a.Impact.ToUpperInvariant() != "POSITIVE"))
             .ForMember(d => d.Trait, s => s.MapFrom(a => a.Traits.FirstOrDefault().Title.ToUpperInvariant()));

            CreateMap<ApiPersonalityFilteringQuestion, DysacFilteringQuestionContentModel>()
              .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
              .ForMember(d => d.Text, s => s.MapFrom(a => a.Text))
              .ForMember(d => d.Skills, s => s.MapFrom(a => ConstructSkills(a.ContentItems)));

            CreateMap<ApiJobProfileOverview, DysacJobProfileOverviewContentModel>()
            .ForMember(d => d.Title, s => s.MapFrom(a => a.CanonicalName.Replace("-", " ")))
            .ForMember(d => d.Id, s => s.MapFrom(a => Guid.NewGuid()))
            .ForMember(d => d.LastCached, s => s.MapFrom(a => DateTime.UtcNow))
            .ForMember(d => d.Html, s => s.MapFrom(a => a.Html));
        }

        private static List<DysacSkillContentItemModel> ConstructSkills(IList<IBaseContentItemModel> contentItems)
        {
            var listToReturn = new List<DysacSkillContentItemModel>();

            var oNetSkills = contentItems.Where(x => x.ContentType == DysacConstants.ContentTypeONetSkill).Select(x => (ApiSkill)x);
            var oNetOccupationCodes = contentItems.Where(x => x.ContentType == DysacConstants.ContentTypeONetOccupationalCode).Select(x => (ApiONetOccupationalCode)x).SelectMany(y => y.ContentItems.Select(z => (ApiSkill)z));

            listToReturn.AddRange(oNetSkills.Union(oNetOccupationCodes).Select(x => new DysacSkillContentItemModel { Description = x.Description, ONetRank = x.ONetRank, Ordinal = x.Ordinal, ItemId = x.ItemId, Title = x.Title, Url = x.Url, LastCached = DateTime.UtcNow }));

            return listToReturn;
        }

        private static List<JobCategoryContentItemModel> ConstructJobCategories(IList<IBaseContentItemModel> z)
        {
            var listToReturn = new List<JobCategoryContentItemModel>();
            var castJobCategories = z.Select(x => (ApiJobCategory)x);

            listToReturn.AddRange(castJobCategories.Select(x => new JobCategoryContentItemModel { Description = x.Description, Ordinal = x.Ordinal, ItemId = x.ItemId, Title = x.Title, Url = x.Url, WebsiteURI = x.WebsiteURI, JobProfiles = ConstructJobProfiles(x.ContentItems), LastCached = DateTime.UtcNow }));

            return listToReturn;
        }

        private static List<JobProfileContentItemModel> ConstructJobProfiles(IList<IBaseContentItemModel> z)
        {
            var listToReturn = new List<JobProfileContentItemModel>();
            var castJobProfiles = z.Select(x => (ApiJobProfile)x);

            listToReturn.AddRange(castJobProfiles.Select(x => new JobProfileContentItemModel { Ordinal = x.Ordinal, ItemId = x.ItemId, Title = x.Title, Url = x.Url, JobProfileWebsiteUrl = x.JobProfileWebsiteUrl, Skills = ConstructSkills(x.ContentItems), LastCached = DateTime.UtcNow }));

            return listToReturn;
        }

        private IEnumerable<DysacShortQuestionContentItemModel> Construct(IList<IBaseContentItemModel> z)
        {
            var listOfQuestions = new List<DysacShortQuestionContentItemModel>();
            var castContentItems = z.Select(x => (ApiShortQuestion)x);

            foreach (var item in castContentItems)
            {
                var question = new DysacShortQuestionContentItemModel { Traits = new List<DysacTraitContentItemModel>(), Ordinal = item.Ordinal, Url = item.Url, Title = item.Title, Impact = item.Impact, ItemId = item.ItemId, LastCached = DateTime.UtcNow };

                var castItems = item.ContentItems.Select(x => (ApiTrait)x);

                question.Traits.AddRange(castItems.Select(z => new DysacTraitContentItemModel { ItemId = z.ItemId, Ordinal = item.Ordinal, Description = z.Description, Title = z.Title.ToUpperInvariant(), Url = z.Url, JobCategories = ConstructJobCategories(z.ContentItems), LastCached = DateTime.UtcNow }));
                listOfQuestions.Add(question);
            }

            return listOfQuestions;
        }
    }
}
