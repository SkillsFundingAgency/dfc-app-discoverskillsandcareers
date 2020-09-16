using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IEventMessageService<TModel>
        where TModel : class, IContentPageModel
    {
        Task<IList<TModel>?> GetAllCachedItemsAsync();

        Task<HttpStatusCode> CreateAsync(TModel? upsertDocumentModel);

        Task<HttpStatusCode> UpdateAsync(TModel? upsertDocumentModel);

        Task<HttpStatusCode> DeleteAsync(Guid id);
    }
}
