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

            if (source!.GetType() == typeof(DysacShortQuestion))
            {
                mapper.Map(child, source as DysacShortQuestion);
            }

            if (source!.GetType() == typeof(DysacSkill))
            {
                mapper.Map(child, source as DysacSkill);
            }

            if (source!.GetType() == typeof(DysacTrait))
            {
                mapper.Map(child, source as DysacTrait);
            }

            if (source!.GetType() == typeof(JobCategory))
            {
                mapper.Map(child, source as JobCategory);
            }

            return source;
        }
    }
}
