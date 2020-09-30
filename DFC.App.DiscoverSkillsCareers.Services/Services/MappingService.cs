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

        public IDysacContentModel Map<TDestination>(TDestination destination, ApiGenericChild child)
            where TDestination : class, IDysacContentModel
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination!.GetType() == typeof(DysacQuestionSetContentModel))
            {
                return mapper.Map(child, destination as DysacQuestionSetContentModel) !;
            }

            if (destination!.GetType() == typeof(DysacShortQuestionContentItemModel))
            {
                return mapper.Map(child, destination as DysacShortQuestionContentItemModel) !;
            }

            if (destination!.GetType() == typeof(DysacSkillContentModel))
            {
                return mapper.Map(child, destination as DysacSkillContentModel) !;
            }

            if (destination!.GetType() == typeof(DysacTraitContentModel))
            {
                return mapper.Map(child, destination as DysacTraitContentModel) !;
            }

            if (destination!.GetType() == typeof(JobCategoryContentItemModel))
            {
                return mapper.Map(child, destination as JobCategoryContentItemModel) !;
            }

            throw new InvalidOperationException($"{destination.GetType().Name} not found");
        }
    }
}
