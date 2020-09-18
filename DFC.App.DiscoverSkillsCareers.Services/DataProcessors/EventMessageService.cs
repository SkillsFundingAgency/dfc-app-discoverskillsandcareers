using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class EventMessageService : IEventMessageService
    {
        private readonly ILogger<EventMessageService> logger;
        private readonly IDocumentServiceFactory documentServiceWrapper;

        public EventMessageService(ILogger<EventMessageService> logger, IDocumentServiceFactory documentServiceWrapper)
        {
            this.logger = logger;
            this.documentServiceWrapper = documentServiceWrapper;
        }

        public async Task<IList<TDestModel>> GetAllCachedItemsAsync<TDestModel>()
            where TDestModel : class, IDysacContentModel
        {
            var serviceDataModels = await GetDocumentService<TDestModel>().GetAllAsync().ConfigureAwait(false);

            return serviceDataModels?.ToList();
        }

        private IDocumentService<TDestModel> GetDocumentService<TDestModel>()
            where TDestModel : class, IDysacContentModel
        {
            return (IDocumentService<TDestModel>)documentServiceWrapper.GetDocumentService<TDestModel>();
        }

        public async Task<HttpStatusCode> CreateAsync<TModel>(TModel upsertDocumentModel)
            where TModel : class, IDysacContentModel
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await GetDocumentService<TModel>().GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var response = await GetDocumentService<TModel>().UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync<TModel>(TModel upsertDocumentModel)
              where TModel : class, IDysacContentModel
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await GetDocumentService<TModel>().GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (existingDocument.PartitionKey != null && existingDocument.PartitionKey.Equals(upsertDocumentModel.PartitionKey, StringComparison.Ordinal))
            {
                upsertDocumentModel.Etag = existingDocument.Etag;
            }
            else
            {
                var deleted = await GetDocumentService<TModel>().DeleteAsync(existingDocument.Id).ConfigureAwait(false);

                if (deleted)
                {
                    logger.LogInformation($"{nameof(UpdateAsync)} has deleted content for: {existingDocument.Id} due to partition key change: {existingDocument.PartitionKey} -> {upsertDocumentModel.PartitionKey}");
                }
                else
                {
                    logger.LogWarning($"{nameof(UpdateAsync)} failed to delete content for: {existingDocument.Id} due to partition key change: {existingDocument.PartitionKey} -> {upsertDocumentModel.PartitionKey}");
                    return HttpStatusCode.BadRequest;
                }
            }

            var response = await GetDocumentService<TModel>().UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(UpdateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync<TModel>(Guid id)
             where TModel : class, IDysacContentModel
        {
            var isDeleted = await GetDocumentService<TModel>().DeleteAsync(id).ConfigureAwait(false);

            if (isDeleted)
            {
                logger.LogInformation($"{nameof(DeleteAsync)} has deleted content for document Id: {id}");
                return HttpStatusCode.OK;
            }
            else
            {
                logger.LogWarning($"{nameof(DeleteAsync)} has returned no content for: {id}");
                return HttpStatusCode.NotFound;
            }
        }
    }
}
