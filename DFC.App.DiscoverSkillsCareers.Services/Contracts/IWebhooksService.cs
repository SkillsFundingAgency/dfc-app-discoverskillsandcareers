using DFC.App.DiscoverSkillsCareers.Models.API;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> DeleteContentAsync<TModel>(TModel destinationType, Guid contentId)
            where TModel : class, IDysacContentModel;

        Task<HttpStatusCode> DeleteContentItemAsync<TModel>(TModel destinationType, Guid contentItemId)
             where TModel : class, IDysacContentModel;

        Task<HttpStatusCode> ProcessContentAsync<TModel, TDestModel>(TModel sourceType, TDestModel destType, Uri url, Guid contentId)
            where TModel : class, IBaseContentItemModel<ApiGenericChild>
            where TDestModel : class, IDysacContentModel;

        Task<HttpStatusCode> ProcessContentItemAsync<TModel>(TModel modelType, Uri url, Guid contentItemId)
             where TModel : class, IDysacContentModel;

        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint, string contentType);

        IDysacContentModel? FindContentItem(Guid contentItemId, List<IDysacContentModel>? items);

        bool RemoveContentItem(Guid contentItemId, List<IDysacContentModel>? items);
    }
}
