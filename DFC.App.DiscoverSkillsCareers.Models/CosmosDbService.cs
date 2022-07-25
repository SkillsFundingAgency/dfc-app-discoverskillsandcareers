using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class CosmosDbService : IDocumentStore
    {
        private const string GetAllSql = "SELECT * FROM c";
        private static readonly QueryDefinition GetAllQuery = new QueryDefinition(GetAllSql);

        public CosmosDbService(
            string assessmentConnectionString,
            string assessmentDatabaseName,
            string assessmentCollectionName,
            string contentConnectionString,
            string contentDatabaseName,
            string contentCollectionName)
        {
            AssessmentClient = new CosmosClient(assessmentConnectionString);
            AssessmentContainer = AssessmentClient
                .GetDatabase(assessmentDatabaseName)
                .GetContainer(assessmentCollectionName);

            ContentClient = new CosmosClient(contentConnectionString);
            ContentContainer = ContentClient
                .GetDatabase(contentDatabaseName)
                .GetContainer(contentCollectionName);
        }

        private Container AssessmentContainer { get; }

        private Container ContentContainer { get; }

        private CosmosClient AssessmentClient { get; }

        private CosmosClient ContentClient { get; }

        public async Task<T?> GetContentByIdAsync<T>(Guid id, string partitionKey)
            where T : class
        {
            const string sql = "SELECT * FROM c WHERE c.id = @id";

            var results = await BaseQuery<T>(
                false,
                partitionKey,
                new QueryDefinition(sql).WithParameter("@id", id.ToString()))
                .ConfigureAwait(false);

            return results.FirstOrDefault();
        }

        public Task<List<T>> GetAllContentAsync<T>(string partitionKey)
            where T : class
        {
            return BaseQuery<T>(false, partitionKey, GetAllQuery);
        }

        public async Task<DysacAssessment?> GetAssessmentAsync(string sessionId)
        {
            const string sql = "SELECT * FROM c WHERE c.id = @sessionId";

            var results = await BaseQuery<DysacAssessment>(
                true,
                new QueryDefinition(sql).WithParameter("@sessionId", sessionId))
                .ConfigureAwait(false);

            return results.FirstOrDefault();
        }

        public async Task UpdateAssessmentAsync(DysacAssessment assessment)
        {
            await AssessmentContainer.UpsertItemAsync(assessment).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> CreateContentAsync<T>(T content)
        {
            var result = await ContentContainer.CreateItemAsync(content).ConfigureAwait(false);
            return result.StatusCode;
        }

        public async Task<HttpStatusCode> UpdateContentAsync<T>(T content)
        {
            var result = await ContentContainer.UpsertItemAsync(content).ConfigureAwait(false);
            return result.StatusCode;
        }

        public async Task<bool> DeleteContentAsync<T>(Guid id, string partitionKey)
            where T : class
        {
            var result = await ContentContainer.DeleteItemAsync<T>(
                id.ToString(),
                new PartitionKey(partitionKey)).ConfigureAwait(false);

            return result.StatusCode == HttpStatusCode.OK;
        }

        private async Task<List<T>> BaseQuery<T>(bool isAssessment, string partitionKey, QueryDefinition query)
            where T : class
        {
            using var iterator = (isAssessment ? AssessmentContainer : ContentContainer).GetItemQueryIterator<T>(
                query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey),
                });

            var returnList = new List<T>();
            while (iterator.HasMoreResults)
            {
                returnList.AddRange(await iterator.ReadNextAsync().ConfigureAwait(false));
            }

            return returnList;
        }

        private async Task<List<T>> BaseQuery<T>(bool isAssessment, QueryDefinition query)
            where T : class
        {
            using var iterator = (isAssessment ? AssessmentContainer : ContentContainer).GetItemQueryIterator<T>(query);

            var returnList = new List<T>();
            while (iterator.HasMoreResults)
            {
                returnList.AddRange(await iterator.ReadNextAsync().ConfigureAwait(false));
            }

            return returnList;
        }
    }
}