using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json.Linq;
using System;
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

            CreateMap<LinkDetails, ApiGenericChild>()
             .ForMember(d => d.Url, s => s.Ignore())
             .ForMember(d => d.ItemId, s => s.Ignore())
             .ForMember(d => d.ContentItems, s => s.Ignore());
        }

        private IEnumerable<DysacShortQuestion> Construct(IList<ApiGenericChild> z)
        {
            var listOfQuestions = new List<DysacShortQuestion>();

            foreach (var item in z)
            {
                var question = new DysacShortQuestion { Traits = new List<DysacTrait>(), Url = item.Url, Title = item.Title, Impact = item.Impact, ItemId = item.ItemId };

                question.Traits.AddRange(item.ContentItems.Select(z => new DysacTrait { ItemId = z.ItemId, Description = z.Description, Title = z.Title, Url = z.Url, ContentItems = BuildJobCategories(z.ContentItems) }));
                listOfQuestions.Add(question);
            }

            return listOfQuestions;
        }

        private List<JobCategory> BuildJobCategories(IList<ApiGenericChild> z)
        {
            return z.Select(x => new JobCategory { Description = x.Description, ItemId = x.ItemId, Title = x.Title, Url = x.Url, WebsiteURI = x.WebsiteURI }).ToList();
        }
    }
}
