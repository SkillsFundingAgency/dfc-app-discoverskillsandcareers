using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class CosmosDbService : IDocumentStore
    {
        private const string GetAllSql = "SELECT * FROM c";
        private static readonly QueryDefinition GetAllQuery = new QueryDefinition(GetAllSql);
        private readonly ILogger<CosmosDbService> logger;

        public CosmosDbService(
            string assessmentConnectionString,
            string assessmentDatabaseName,
            string assessmentCollectionName,
            string contentConnectionString,
            string contentDatabaseName,
            string contentCollectionName,
            ILogger<CosmosDbService> logger,
            CosmosDbAppInsightsRequestHandler assessmentRequestHandler,
            CosmosDbAppInsightsRequestHandler contentRequestHandler)
        {
            ContentClient = new CosmosClient(contentConnectionString, new CosmosClientOptions
            {
                CustomHandlers = { contentRequestHandler },
            });

            AssessmentClient = new CosmosClient(assessmentConnectionString, new CosmosClientOptions
            {
                CustomHandlers = { assessmentRequestHandler },
            });

            AssessmentContainer = AssessmentClient
                .GetDatabase(assessmentDatabaseName)
                .GetContainer(assessmentCollectionName);

            ContentContainer = ContentClient
                .GetDatabase(contentDatabaseName)
                .GetContainer(contentCollectionName);

            this.logger = logger;
        }

        private Container AssessmentContainer { get; }

        private Container ContentContainer { get; }

        private CosmosClient AssessmentClient { get; }

        private CosmosClient ContentClient { get; }

        public async Task<T?> GetContentByIdAsync<T>(Guid id, string partitionKey, [CallerMemberName] string callerMemberName = "")
            where T : class
        {
            const string sql = "SELECT * FROM c WHERE c.id = @id";

            var results = await BaseQuery<T>(
                false,
                partitionKey,
                new QueryDefinition(sql).WithParameter("@id", id.ToString()),
                ContainerName.Content,
                callerMemberName,
                id.ToString())
                .ConfigureAwait(false);

            return results.FirstOrDefault();
        }

        public Task<List<T>> GetAllContentAsync<T>(string partitionKey, [CallerMemberName] string callerMemberName = "")
            where T : class
        {
            return BaseQuery<T>(false, partitionKey, GetAllQuery, ContainerName.Content, callerMemberName, "N/A");
        }

        public async Task<DysacAssessment?> GetAssessmentAsync(string sessionId, [CallerMemberName] string callerMemberName = "")
        {
            const string sql = "SELECT * FROM c WHERE c.id = @sessionId";

            var results = await BaseQuery<DysacAssessment>(
                true,
                new QueryDefinition(sql).WithParameter("@sessionId", sessionId),
                ContainerName.Assessment,
                callerMemberName,
                sessionId)
                .ConfigureAwait(false);

            return results.FirstOrDefault();
        }

        public async Task UpdateAssessmentAsync(DysacAssessment assessment, [CallerMemberName] string callerMemberName = "")
        {
            var result = await AssessmentContainer.UpsertItemAsync(assessment).ConfigureAwait(false);
            LogCosmosStats(
                result.RequestCharge,
                result.Diagnostics.GetClientElapsedTime(),
                QueryTypes.Upsert,
                ContainerName.Assessment,
                assessment.PartitionKey ?? "N/A",
                callerMemberName,
                "N/A",
                "N/A");
        }

        public async Task<HttpStatusCode> CreateContentAsync<T>(T content, [CallerMemberName] string callerMemberName = "")
        {
            var result = await ContentContainer.CreateItemAsync(content).ConfigureAwait(false);
            LogCosmosStats(
                result.RequestCharge,
                result.Diagnostics.GetClientElapsedTime(),
                QueryTypes.Create,
                ContainerName.Content,
                "N/A",
                callerMemberName,
                "N/A",
                "N/A");
            return result.StatusCode;
        }

        public async Task<HttpStatusCode> UpdateContentAsync<T>(T content, [CallerMemberName] string callerMemberName = "")
        {
            var result = await ContentContainer.UpsertItemAsync(content).ConfigureAwait(false);
            LogCosmosStats(
                result.RequestCharge,
                result.Diagnostics.GetClientElapsedTime(),
                QueryTypes.Upsert,
                ContainerName.Content,
                "N/A",
                callerMemberName,
                "N/A",
                "N/A");

            return result.StatusCode;
        }

        public async Task<bool> DeleteContentAsync<T>(Guid id, string partitionKey, [CallerMemberName] string callerMemberName = "")
            where T : class
        {
            var result = await ContentContainer.DeleteItemAsync<T>(
                id.ToString(),
                new PartitionKey(partitionKey)).ConfigureAwait(false);

            LogCosmosStats(
                result.RequestCharge,
                result.Diagnostics.GetClientElapsedTime(),
                QueryTypes.Delete,
                ContainerName.Content,
                partitionKey,
                callerMemberName,
                "N/A",
                id.ToString());

            return result.StatusCode == HttpStatusCode.OK;
        }

        private async Task<List<T>> BaseQuery<T>(bool isAssessment, string partitionKey, QueryDefinition query, ContainerName containerName, string callerMemberName, string queryParameter)
            where T : class
        {
            using var iterator = (isAssessment ? AssessmentContainer : ContentContainer).GetItemQueryIterator<T>(
                query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey),
                });

            return await RunIterator(iterator, containerName, partitionKey, callerMemberName, query.QueryText, queryParameter).ConfigureAwait(false);
        }

        private async Task<List<T>> BaseQuery<T>(bool isAssessment, QueryDefinition query, ContainerName containerName, string callerMemberName, string queryParameter)
            where T : class
        {
            using var iterator = (isAssessment ? AssessmentContainer : ContentContainer).GetItemQueryIterator<T>(query);

            return await RunIterator(iterator, containerName, "N/A", callerMemberName, query.QueryText, queryParameter).ConfigureAwait(false);
        }

        private async Task<List<T>> RunIterator<T>(FeedIterator<T> iterator, ContainerName containerName, string partitionKey, string callerMemberName, string queryText, string queryParameter)
            where T : class
        {
            var requestCharge = 0d;
            TimeSpan elapsedTime = new();

            var returnList = new List<T>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync().ConfigureAwait(false);
                returnList.AddRange(response);
                requestCharge += response.RequestCharge;
                elapsedTime += new TimeSpan(response.Diagnostics.GetClientElapsedTime().Ticks);
            }

            LogCosmosStats(requestCharge, elapsedTime, QueryTypes.QueryDefinition, containerName, partitionKey, callerMemberName, queryText, queryParameter);

            return returnList;
        }

        private void LogCosmosStats(
            double requestCharge,
            TimeSpan elapsedTime,
            QueryTypes queryType,
            ContainerName containerName,
            string partitionKey,
            string callerMemberName,
            string query,
            string queryParameter)
        {
            try
            {
                logger.LogInformation($"CallerMemberName: {callerMemberName}, Container: {containerName}, QueryType: {queryType}, Query: {query}, QueryParameter: {queryParameter}, RequestCharge: {requestCharge}, PartitionKey: {partitionKey}, ElapsedTime: {elapsedTime}");
            }
            catch (Exception exception)
            {
                logger.LogError($"Error occurred while logging. Exception: {exception}");
            }
        }
    }
}
