using DFC.App.DiscoverSkillsCareers.Core.Constants;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Repositories
{
    public class QuestionSetRepository : IQuestionSetRepository
    {
        private readonly DocumentClient documentClient;
        private readonly ICosmosSettings cosmosSettings;
        private readonly string collectionName = ContainerName.QuestionSets;

        public QuestionSetRepository(DocumentClient documentClient, ICosmosSettings cosmosSettings)
        {
            this.documentClient = documentClient;
            this.cosmosSettings = cosmosSettings;
        }

        public async Task<QuestionSet> GetCurrentQuestionSet(string assessmentType)
        {
            var feedOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey("latest-questionset"),
            };

            var uri = UriFactory.CreateDocumentUri(cosmosSettings.DatabaseName, collectionName, $"latest-{assessmentType}");
            QuestionSet qs = null;

            try
            {
                var result = await documentClient.ReadDocumentAsync<QuestionSet>(uri, feedOptions).ConfigureAwait(false);
                qs = result.Document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var collectionUri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                    QuestionSet latestQs = documentClient.CreateDocumentQuery<QuestionSet>(collectionUri, new FeedOptions() { EnableCrossPartitionQuery = true })
                                   .Where(x => x.AssessmentType == assessmentType && x.IsCurrent)
                                   .OrderByDescending(x => x.Version)
                                   .AsEnumerable()
                                   .FirstOrDefault();

                    await CreateUpdateLatestQuestionSet(latestQs).ConfigureAwait(false);

                    qs = latestQs;
                }
            }

            if (qs != null)
            {
                qs.QuestionSetVersion = $"{assessmentType.ToLower()}-{qs.Title.ToLower()}-{qs.Version.ToString()}";
                qs.PartitionKey = "ncs";
            }

            return qs;
        }

        private async Task CreateUpdateLatestQuestionSet(QuestionSet questionSet)
        {
            if (questionSet?.IsCurrent == true)
            {
                questionSet.PartitionKey = "latest-questionset";
                questionSet.QuestionSetVersion = $"latest-{questionSet.AssessmentType}";

                var uri = UriFactory.CreateDocumentCollectionUri(cosmosSettings.DatabaseName, collectionName);
                await documentClient.UpsertDocumentAsync(uri, questionSet).ConfigureAwait(false);
            }
        }
    }
}
