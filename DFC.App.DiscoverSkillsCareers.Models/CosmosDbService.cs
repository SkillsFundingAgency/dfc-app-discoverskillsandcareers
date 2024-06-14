using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using DFC.App.DiscoverSkillsCareers.Models.Enums;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    [ExcludeFromCodeCoverage]
    public class CosmosDbService : IDocumentStore
    {
        private readonly ILogger<CosmosDbService> logger;

        public CosmosDbService(
            string assessmentConnectionString,
            string assessmentDatabaseName,
            string assessmentCollectionName,
            ILogger<CosmosDbService> logger,
            CosmosDbAppInsightsRequestHandler assessmentRequestHandler)
        {
            AssessmentClient = new CosmosClient(assessmentConnectionString, new CosmosClientOptions
            {
                CustomHandlers = { assessmentRequestHandler },
            });

            AssessmentContainer = AssessmentClient
                .GetDatabase(assessmentDatabaseName)
                .GetContainer(assessmentCollectionName);

            this.logger = logger;
        }

        private Container AssessmentContainer { get; }

        private CosmosClient AssessmentClient { get; }

        public async Task<DysacAssessment?> GetAssessmentAsync(string sessionId, [CallerMemberName] string callerMemberName = "")
        {
            const string sql = "SELECT * FROM c WHERE c.id = @sessionId";

            var results = await BaseQuery<DysacAssessment>(
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

        private async Task<List<T>> BaseQuery<T>(QueryDefinition query, ContainerName containerName, string callerMemberName, string queryParameter)
            where T : class
        {
            using var iterator = AssessmentContainer.GetItemQueryIterator<T>(query);

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
