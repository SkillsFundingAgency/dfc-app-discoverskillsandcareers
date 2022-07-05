using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class EventMessageService : IEventMessageService
    {
        private readonly ILogger<EventMessageService> logger;
        private readonly IDocumentStore documentStore;

        public EventMessageService(ILogger<EventMessageService> logger, IDocumentStore documentStore)
        {
            this.logger = logger;
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<IEnumerable<TDestModel>?> GetAllCachedItemsAsync<TDestModel>()
            where TDestModel : class, IDocumentModel
        {
            var itemInstance = (TDestModel?)Activator.CreateInstance(typeof(TDestModel));

            if (itemInstance == null)
            {
                throw new ArgumentException($"{typeof(TDestModel)} is null");
            }

            var serviceDataModels = await documentStore.GetAllContentAsync<TDestModel>(
                itemInstance.PartitionKey!).ConfigureAwait(false);

            return serviceDataModels;
        }

        public async Task<HttpStatusCode> CreateAsync<TModel>(TModel? upsertDocumentModel)
            where TModel : class, IDocumentModel
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await documentStore.GetContentByIdAsync<TModel>(
                upsertDocumentModel.Id,
                upsertDocumentModel.PartitionKey!).ConfigureAwait(false);

            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var response = await documentStore.CreateContentAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync<TModel>(TModel? upsertDocumentModel)
              where TModel : class, IDocumentModel
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await documentStore.GetContentByIdAsync<TModel>(
                upsertDocumentModel.Id,
                upsertDocumentModel.PartitionKey!).ConfigureAwait(false);

            if (existingDocument == null)
            {
                return HttpStatusCode.NotFound;
            }

            if (existingDocument.PartitionKey != null
                && existingDocument.PartitionKey.Equals(upsertDocumentModel.PartitionKey, StringComparison.Ordinal))
            {
                upsertDocumentModel.Etag = existingDocument.Etag;
            }
            else
            {
                var deleted = await documentStore.DeleteContentAsync<TModel>(existingDocument.Id, existingDocument.PartitionKey!).ConfigureAwait(false);

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

            var response = await documentStore.UpdateContentAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(UpdateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync<TModel>(Guid id, string partitionKey)
             where TModel : class, IDocumentModel
        {
            var isDeleted = await documentStore.DeleteContentAsync<TModel>(id, partitionKey).ConfigureAwait(false);

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
