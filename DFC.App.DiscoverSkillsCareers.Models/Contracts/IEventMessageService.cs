using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IEventMessageService
    {
        Task<IList<TDestModel>?> GetAllCachedItemsAsync<TDestModel>()
            where TDestModel : class, IDysacContentModel;

        Task<HttpStatusCode> CreateAsync<TModel>(TModel upsertDocumentModel)
            where TModel : class, IDysacContentModel;

        Task<HttpStatusCode> UpdateAsync<TModel>(TModel upsertDocumentModel)
            where TModel : class, IDysacContentModel;

        Task<HttpStatusCode> DeleteAsync<TModel>(Guid id)
             where TModel : class, IDysacContentModel;
    }
}
