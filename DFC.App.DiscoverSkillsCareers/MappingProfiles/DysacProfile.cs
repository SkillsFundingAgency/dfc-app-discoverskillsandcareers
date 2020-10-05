using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
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
            CreateMap<LinkDetails, DysacShortQuestionContentItemModel>();
            CreateMap<LinkDetails, ApiShortQuestion>();
            CreateMap<LinkDetails, ApiTrait>();
            CreateMap<LinkDetails, ApiSkill>();
            CreateMap<LinkDetails, ApiJobCategory>();
            CreateMap<LinkDetails, ApiJobProfile>();

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

            CreateMap<DysacShortQuestionContentItemModel, ShortQuestion>()
             .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
             .ForMember(d => d.QuestionText, s => s.MapFrom(a => a.Title))
             .ForMember(d => d.IsNegative, s => s.MapFrom(a => a.Impact.ToUpperInvariant() != "POSITIVE"))
             .ForMember(d => d.Trait, s => s.MapFrom(a => a.Title.ToUpperInvariant()));

            CreateMap<ApiPersonalityFilteringQuestion, DysacFilteringQuestionContentModel>()
              .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
              .ForMember(d => d.Text, s => s.MapFrom(a => a.Title))
              .ForMember(d => d.Skills, s => s.MapFrom(a => ConstructSkills(a.ContentItems)));

        }

        private static List<DysacSkillContentItemModel> ConstructSkills(IList<IBaseContentItemModel> contentItems)
        {
            var listToReturn = new List<DysacSkillContentItemModel>();
            var castSkills = contentItems.Select(x => (ApiSkill)x);

            listToReturn.AddRange(castSkills.Select(x => new DysacSkillContentItemModel { Description = x.Description, Ordinal = x.Ordinal, ItemId = x.ItemId, Title = x.Title, Url = x.Url, LastCached = DateTime.UtcNow }));

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
