﻿using DFC.App.DiscoverSkillsCareers.Models.API;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IMappingService
    {
        IDysacContentModel Map<TDestination>(TDestination destination, ApiGenericChild child)
            where TDestination : class, IDysacContentModel;
    }
}
