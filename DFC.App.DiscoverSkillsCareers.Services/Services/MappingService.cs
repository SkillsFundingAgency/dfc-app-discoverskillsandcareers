using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;

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
            if (source!.GetType() == typeof(DysacShortQuestion))
            {
                mapper.Map(child, source as DysacShortQuestion);
            }

            return source;
        }
    }
}
