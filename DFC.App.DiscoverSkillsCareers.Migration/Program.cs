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
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();

            var cosmosDbConnectionLegacyUserSessions = configuration.GetSection("Configuration:CosmosDbConnections:LegacySessions").Get<CosmosDbConnection>();

            Console.WriteLine("Do you wish to populate test data in the old DYSAC or migrate data? Please enter either 'populatetest' or 'migrate'.");
            var inputMode = Console.ReadLine();
            
            if (!inputMode.Equals("populatetest", StringComparison.InvariantCultureIgnoreCase)
                && !inputMode.Equals("migrate", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Input mode '{inputMode}' not understood. Quitting.");
                return;
            }
            
            Console.WriteLine("Cosmos Db Connection mode: Please enter the Cosmos Db connection mode to use (gateway or direct - defaults to gateway)");
            var cosmosDbConnectionMode = Console.ReadLine()!.Trim();

            if (!cosmosDbConnectionMode.Equals("direct", StringComparison.InvariantCultureIgnoreCase))
            {
                cosmosDbConnectionMode = "gateway";
            }

            var connectionMode = cosmosDbConnectionMode == "direct" ? ConnectionMode.Direct : ConnectionMode.Gateway;

            Console.WriteLine("Cosmos Db destination RUs: Please enter the RUs for the destination container.");
            var cosmosDbDestinationRUs = int.Parse(Console.ReadLine()!.Trim());
            
            if (inputMode.Equals("populatetest", StringComparison.InvariantCultureIgnoreCase))
            {
                await PopulateTestData(cosmosDbConnectionLegacyUserSessions, connectionMode, cosmosDbDestinationRUs);
                return;
            }

            var cosmosDbConnectionContent = configuration.GetSection("Configuration:CosmosDbConnections:DysacContent").Get<CosmosDbConnection>();
            var cosmosDbConnectionAssessment = configuration.GetSection("Configuration:CosmosDbConnections:DysacAssessment").Get<CosmosDbConnection>();
            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDocumentServices<DysacTraitContentModel>(cosmosDbConnectionContent, true, cosmosRetryOptions)
                .AddDocumentServices<DysacAssessment>(cosmosDbConnectionAssessment, true, cosmosRetryOptions)
                .AddDocumentServices<DysacFilteringQuestionContentModel>(cosmosDbConnectionContent, true, cosmosRetryOptions)
                .AddDocumentServices<DysacQuestionSetContentModel>(cosmosDbConnectionContent, true, cosmosRetryOptions)
                .AddSingleton(configuration.GetSection(nameof(MigrationOptions)).Get<MigrationOptions>() ?? new MigrationOptions())
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
                    ConnectionMode = connectionMode,
                    ConnectionProtocol = Protocol.Tcp
                });

            var migrationService = new MigrationService(
                questionSetDocumentService,
                filteringQuestionDocumentService,
                userSessionDocumentService,
                dysacQuestionSetDocumentService,
                destinationDocumentClient,
                cosmosDbConnectionAssessment.DatabaseId,
                cosmosDbConnectionAssessment.CollectionId,
                cosmosDbDestinationRUs);
            
            Console.Write(@"Please check and confirm the indexing strategy for the cosmos destination is set as per the below, or the import may fail.;

{
    ""indexingMode"": ""none"",
    ""automatic"": false,
    ""includedPaths"": [],
    ""excludedPaths"": []
}

Please ensure you set it back after to;

{
    ""indexingMode"": ""consistent"",
    ""automatic"": true,
    ""includedPaths"": [
        {
            ""path"": ""/*""
        }
    ],
    ""excludedPaths"": [
        {
            ""path"": ""/\""_etag\""/?""
        }
    ]
}

Press y to proceed if you are happy this has been done.
");
            
            Console.ReadKey();
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please also confirm you are running standalone and not debugging - as that will make this process very slow. Press y to proceed");
            Console.ReadKey();

            Console.WriteLine();
            Console.WriteLine();
            
            Activity.Current = new Activity("Dysac assessment migration").Start();
            await migrationService.Start();
        }

        private static async Task PopulateTestData(CosmosDbConnection cosmosDbConnectionLegacyUserSessions, ConnectionMode connectionMode, int cosmosDbDestinationRUs)
        {
            var sourceDocumentClient = new DocumentClient(
                cosmosDbConnectionLegacyUserSessions.EndpointUrl,
                cosmosDbConnectionLegacyUserSessions.AccessKey,
                new ConnectionPolicy
                {
                    MaxConnectionLimit = 1000,
                    ConnectionMode = connectionMode,
                    ConnectionProtocol = Protocol.Tcp
                });
                
            var migrationService = new PopulateTestDataService(sourceDocumentClient, cosmosDbDestinationRUs);

            Activity.Current = new Activity("Dysac test data population").Start();
            await migrationService.Start();
        }
    }
}