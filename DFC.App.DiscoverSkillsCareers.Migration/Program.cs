using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Migration.Models;
using DFC.App.DiscoverSkillsCareers.Migration.Services;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Migration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();

            var cosmosDbConnectionContent = Configuration.GetSection("Configuration:CosmosDbConnections:DysacContent").Get<CosmosDbConnection>();
            var cosmosDbConnectionAssessment = Configuration.GetSection("Configuration:CosmosDbConnections:DysacAssessment").Get<CosmosDbConnection>();
            var cosmosDbConnectionLegacyUserSessions = Configuration.GetSection("Configuration:CosmosDbConnections:LegacySessions").Get<CosmosDbConnection>();
            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDocumentServices<DysacTraitContentModel>(cosmosDbConnectionContent, true, cosmosRetryOptions)
                .AddDocumentServices<DysacAssessment>(cosmosDbConnectionAssessment, true, cosmosRetryOptions)
                .AddDocumentServices<DysacFilteringQuestionContentModel>(cosmosDbConnectionContent, true, cosmosRetryOptions)
                .AddDocumentServices<DysacQuestionSetContentModel>(cosmosDbConnectionContent, true, cosmosRetryOptions)
                .AddSingleton(Configuration.GetSection(nameof(MigrationOptions)).Get<MigrationOptions>() ?? new MigrationOptions())
                .AddSingleton<IDocumentClient>(
                    new DocumentClient(cosmosDbConnectionLegacyUserSessions.EndpointUrl,
                    cosmosDbConnectionLegacyUserSessions.AccessKey))
                .AddAutoMapper(typeof(Program))
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            var questionSetDocumentService = serviceProvider.GetService<IDocumentService<DysacTraitContentModel>>();
            var filteringQuestionDocumentService = serviceProvider.GetService<IDocumentService<DysacFilteringQuestionContentModel>>();
            var dysacQuestionSetDocumentService = serviceProvider.GetService<IDocumentService<DysacQuestionSetContentModel>>();
            var userSessionDocumentService = serviceProvider.GetService<IDocumentClient>();
            
            var destinationDocumentClient = new DocumentClient(
                cosmosDbConnectionAssessment.EndpointUrl,
                cosmosDbConnectionAssessment.AccessKey,
                new ConnectionPolicy
                {
                    MaxConnectionLimit = 1000,
                    ConnectionMode = ConnectionMode.Direct, // Be careful on VPN
                    ConnectionProtocol = Protocol.Tcp
                });

            var migrationService = new MigrationService(
                questionSetDocumentService,
                filteringQuestionDocumentService,
                userSessionDocumentService,
                dysacQuestionSetDocumentService,
                destinationDocumentClient,
                cosmosDbConnectionAssessment.DatabaseId,
                cosmosDbConnectionAssessment.CollectionId);

            // Uncomment out the next block and comment out the ones above if wanting to populate data
            /*var sourceDocumentClient = new DocumentClient(
                cosmosDbConnectionLegacyUserSessions.EndpointUrl,
                cosmosDbConnectionLegacyUserSessions.AccessKey,
                new ConnectionPolicy
                {
                    MaxConnectionLimit = 1000,
                    ConnectionMode = ConnectionMode.Direct, // Be careful on VPN
                    ConnectionProtocol = Protocol.Tcp
                });
            var migrationService = new PopulateTestDataService(sourceDocumentClient);*/
            
            Activity.Current = new Activity("Dysac Assessment Migration").Start();
            await migrationService.Start();

            Console.WriteLine("Completed - press a key to end");
            Console.ReadKey();
        }
    }
}
