﻿using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
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
        private readonly IDocumentServiceFactory documentServiceFactory;

        public EventMessageService(ILogger<EventMessageService> logger, IDocumentServiceFactory documentServiceWrapper)
        {
            this.logger = logger;
            this.documentServiceFactory = documentServiceWrapper;
        }

        public async Task<IEnumerable<TDestModel>?> GetAllCachedItemsAsync<TDestModel>()
            where TDestModel : class, IDocumentModel
        {
            var itemInstance = (TDestModel?)Activator.CreateInstance(typeof(TDestModel));

            if (itemInstance == null)
            {
                throw new ArgumentException($"{typeof(TDestModel)} is null");
            }

            var serviceDataModels = await documentServiceFactory.GetDocumentService<TDestModel>().GetAsync(x => x.PartitionKey == itemInstance!.PartitionKey).ConfigureAwait(false);

            return serviceDataModels;
        }

        public async Task<HttpStatusCode> CreateAsync<TModel>(TModel upsertDocumentModel)
            where TModel : class, IDocumentModel
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await documentServiceFactory.GetDocumentService<TModel>().GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var response = await documentServiceFactory.GetDocumentService<TModel>().UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> UpdateAsync<TModel>(TModel upsertDocumentModel)
              where TModel : class, IDocumentModel
        {
            if (upsertDocumentModel == null)
            {
                return HttpStatusCode.BadRequest;
            }

            var existingDocument = await documentServiceFactory.GetDocumentService<TModel>().GetByIdAsync(upsertDocumentModel.Id).ConfigureAwait(false);
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
                var deleted = await documentServiceFactory.GetDocumentService<TModel>().DeleteAsync(existingDocument.Id).ConfigureAwait(false);

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

            var response = await documentServiceFactory.GetDocumentService<TModel>().UpsertAsync(upsertDocumentModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(UpdateAsync)} has upserted content for: {upsertDocumentModel.Id} with response code {response}");

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync<TModel>(Guid id)
             where TModel : class, IDocumentModel
        {
            var isDeleted = await documentServiceFactory.GetDocumentService<TModel>().DeleteAsync(id).ConfigureAwait(false);

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
