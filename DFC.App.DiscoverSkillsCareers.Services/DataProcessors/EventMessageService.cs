using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.DataProcessors
{
    public class EventMessageService<TModel> : IEventMessageService<TModel>
           where TModel : class, IDocumentModel
    {
        private readonly ILogger<EventMessageService<TModel>> logger;
        private readonly IDocumentService<TModel> contentPageService;

        public EventMessageService(ILogger<EventMessageService<TModel>> logger, IDocumentService<TModel> contentPageService)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
        }

        public async Task<IList<TModel>?> GetAllCachedItemsAsync()
        {
            var serviceDataModels = await contentPageService.GetAllAsync().ConfigureAwait(false);

            return serviceDataModels?.ToList();
        }

        public async Task<HttpStatusCode> CreateAsync(TModel? upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await contentPageService.GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var response = await contentPageService.UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync(TModel? upsertDocumentModel)
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await contentPageService.GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
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
                var deleted = await contentPageService.DeleteAsync(existingDocument.Id).ConfigureAwait(false);

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

            var response = await contentPageService.UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(UpdateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            var isDeleted = await contentPageService.DeleteAsync(id).ConfigureAwait(false);

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
