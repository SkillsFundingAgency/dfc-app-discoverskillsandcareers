using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IEventMessageService
    {
        Task<IEnumerable<TDestModel>?> GetAllCachedItemsAsync<TDestModel>()
            where TDestModel : class, IDocumentModel;

        Task<HttpStatusCode> CreateAsync<TModel>(TModel upsertDocumentModel)
            where TModel : class, IDocumentModel;

        Task<HttpStatusCode> UpdateAsync<TModel>(TModel upsertDocumentModel)
            where TModel : class, IDocumentModel;

        Task<HttpStatusCode> DeleteAsync<TModel>(Guid id)
             where TModel : class, IDocumentModel;
    }
}
