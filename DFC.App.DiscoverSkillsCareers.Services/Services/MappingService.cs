using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using System;

namespace DFC.App.DiscoverSkillsCareers.Services.Services
{
    public class MappingService : IMappingService
    {
        private readonly IMapper mapper;

        public MappingService(AutoMapper.IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IDysacContentModel Map<TDestination>(TDestination source, ApiGenericChild child)
            where TDestination : class, IDysacContentModel
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source!.GetType() == typeof(DysacQuestionSetContentModel))
            {
                mapper.Map(child, source as DysacQuestionSetContentModel);
            }

            if (source!.GetType() == typeof(DysacShortQuestionContentItemModel))
            {
                mapper.Map(child, source as DysacShortQuestionContentItemModel);
            }

            if (source!.GetType() == typeof(DysacSkilContentModell))
            {
                mapper.Map(child, source as DysacSkilContentModell);
            }

            if (source!.GetType() == typeof(DysacTraitContentModel))
            {
                mapper.Map(child, source as DysacTraitContentModel);
            }

            if (source!.GetType() == typeof(JobCategoryContentItemModel))
            {
                mapper.Map(child, source as JobCategoryContentItemModel);
            }

            return source;
        }
    }
}
