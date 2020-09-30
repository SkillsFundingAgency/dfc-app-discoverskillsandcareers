using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.DiscoverSkillsCareers.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class DysacProfile : Profile
    {
        public DysacProfile()
        {
            CreateMap<ApiQuestionSet, DysacQuestionSetContentModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.ShortQuestions, s => s.MapFrom(z => Construct(z.ContentItems)));

            CreateMap<ApiTrait, DysacTraitContentModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.JobCategories, s => s.MapFrom(z => ConstructJobCategories(z.ContentItems)));

            CreateMap<ApiSkill, DysacSkillContentModel>()
              .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId));

            CreateMap<LinkDetails, ApiGenericChild>()
             .ForMember(d => d.Url, s => s.Ignore())
             .ForMember(d => d.ItemId, s => s.Ignore())
             .ForMember(d => d.ContentItems, s => s.Ignore());

            CreateMap<ApiGenericChild, DysacQuestionSetContentModel>();
            CreateMap<ApiGenericChild, DysacShortQuestionContentItemModel>();
            CreateMap<ApiGenericChild, DysacSkillContentModel>();
            CreateMap<ApiGenericChild, DysacTraitContentModel>();
            CreateMap<ApiGenericChild, JobCategoryContentItemModel>();
        }

        private static List<IDysacContentModel> ConstructJobCategories(IList<ApiGenericChild> z)
        {
            var listToReturn = new List<IDysacContentModel>();
            listToReturn.AddRange(z.Select(x => new JobCategoryContentItemModel { Description = x.Description, ItemId = x.ItemId, Title = x.Title, Url = x.Url, WebsiteURI = x.WebsiteURI }));

            return listToReturn;
        }

        private IEnumerable<DysacShortQuestionContentItemModel> Construct(IList<ApiGenericChild> z)
        {
            var listOfQuestions = new List<DysacShortQuestionContentItemModel>();

            foreach (var item in z)
            {
                var question = new DysacShortQuestionContentItemModel { Traits = new List<IDysacContentModel>(), Url = item.Url, Title = item.Title, Impact = item.Impact, ItemId = item.ItemId };

                question.Traits.AddRange(item.ContentItems.Select(z => new DysacTraitContentItemModel { ItemId = z.ItemId, Description = z.Description, Title = z.Title, Url = z.Url, JobCategories = ConstructJobCategories(z.ContentItems) }));
                listOfQuestions.Add(question);
            }

            return listOfQuestions;
        }
    }
}
