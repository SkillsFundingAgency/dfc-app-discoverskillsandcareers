using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Migration.Models;
using DFC.App.DiscoverSkillsCareers.Migration.Services;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Models.Contracts;
using Microsoft.Azure.Cosmos;

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

            //run only with error list file
            Console.WriteLine("Use error list file to run (yes or no)");
            var runfromerrorfileonly = 'y' == Console.ReadLine()!.Trim().ToLower()[0];
            var inputMode = "migrate";
            var connectionMode = ConnectionMode.Direct;
            var cosmosDbDestinationRUs = 10000;
            var cosmosDbConnectionMode = "direct";
            var useBookmark = false;
            DateTime? cutoffDateTime = null;
            if (!runfromerrorfileonly)
            {
                Console.WriteLine("Do you wish to populate test data in the old DYSAC or migrate data? Please enter either 'populatetest' or 'migrate'.");
                inputMode = Console.ReadLine();

                if (inputMode?.Equals("populatetest", StringComparison.InvariantCultureIgnoreCase) != true
                    && inputMode?.Equals("migrate", StringComparison.InvariantCultureIgnoreCase) != true)
                {
                    Console.WriteLine($"Input mode '{inputMode}' not understood. Quitting.");
                    return;
                }

                Console.WriteLine("Cosmos Db Connection mode: Please enter the Cosmos Db connection mode to use (gateway or direct - defaults to gateway)");
                cosmosDbConnectionMode = Console.ReadLine()!.Trim();

                if (!cosmosDbConnectionMode.Equals("direct", StringComparison.InvariantCultureIgnoreCase))
                {
                    cosmosDbConnectionMode = "gateway";
                }

                connectionMode = cosmosDbConnectionMode == "direct" ? ConnectionMode.Direct : ConnectionMode.Gateway;

                Console.WriteLine("Cosmos Db destination RUs: Please enter the RUs for the destination container.");
                cosmosDbDestinationRUs = int.Parse(Console.ReadLine()!.Trim());

                Console.WriteLine("Use recovery bookmark: Whether or not to restart from the bookmark if a previous run failed (yes or no)");
                useBookmark = 'y' == Console.ReadLine()!.Trim().ToLower()[0];

                Console.WriteLine("Cut off date: The date to take assessments up to (useful for a 'catch up' run of the tool - in format yyyy-MM-dd hh:mm:ss)");
                var cutoffDateTimeString = Console.ReadLine()!.Trim();

                if (DateTime.TryParse(cutoffDateTimeString, out var cutoffDateTimeTemp))
                {
                    cutoffDateTime = cutoffDateTimeTemp;
                }
            }
            var cosmosDbConnectionAssessment = configuration.GetSection("Configuration:CosmosDbConnections:DysacAssessment")
                .Get<CosmosDbConnection>();

            var cosmosDbConnectionContent = configuration.GetSection("Configuration:CosmosDbConnections:DysacContent")
                .Get<CosmosDbConnection>();

            Console.WriteLine();
            Console.WriteLine("Summary:");
            Console.WriteLine();

            Console.WriteLine($"Mode: {inputMode}");
            Console.WriteLine($"Connection mode: {cosmosDbConnectionMode}");
            Console.WriteLine($"Destination RUs: {cosmosDbDestinationRUs}");
            Console.WriteLine($"Source sessions endpoint: {cosmosDbConnectionLegacyUserSessions.EndpointUrl} {cosmosDbConnectionLegacyUserSessions.DatabaseId} {cosmosDbConnectionLegacyUserSessions.CollectionId}");
            Console.WriteLine($"Destination sessions endpoint: {cosmosDbConnectionAssessment.EndpointUrl} {cosmosDbConnectionAssessment.DatabaseId} {cosmosDbConnectionAssessment.CollectionId}");
            Console.WriteLine($"Content endpoint: {cosmosDbConnectionContent.EndpointUrl} {cosmosDbConnectionContent.DatabaseId} {cosmosDbConnectionContent.CollectionId}");
            Console.WriteLine($"Use recovery bookmark: {useBookmark}");
            Console.WriteLine($"Cut off date: {cutoffDateTime}");
            Console.WriteLine();

            Console.WriteLine("Press any key to proceed.");
            Console.ReadKey();

            if (inputMode.Equals("populatetest", StringComparison.InvariantCultureIgnoreCase))
            {
                await PopulateTestData(cosmosDbConnectionLegacyUserSessions, connectionMode, cosmosDbDestinationRUs);
                return;
            }

            var services = new ServiceCollection()
                 .AddLogging()
                 .AddSingleton(configuration.GetSection(nameof(MigrationOptions)).Get<MigrationOptions>() ??
                               new MigrationOptions())
                 .AddSingleton<Microsoft.Azure.Documents.IDocumentClient>(
                    new Microsoft.Azure.Documents.Client.DocumentClient(cosmosDbConnectionLegacyUserSessions.EndpointUrl,
                        cosmosDbConnectionLegacyUserSessions.AccessKey))
                 .AddAutoMapper(typeof(Program));

            services.AddSingleton<IDocumentStore, CosmosDbService>(_ =>
            {
                var connectionStringAssessment = $"AccountEndpoint={cosmosDbConnectionAssessment.EndpointUrl};AccountKey={cosmosDbConnectionAssessment.AccessKey};";
                var connectionStringContent = $"AccountEndpoint={cosmosDbConnectionContent.EndpointUrl};AccountKey={cosmosDbConnectionContent.AccessKey};";

                return new CosmosDbService(
                    connectionStringAssessment,
                    cosmosDbConnectionAssessment.DatabaseId!,
                    cosmosDbConnectionAssessment.CollectionId!,
                    connectionStringContent,
                    cosmosDbConnectionContent.DatabaseId!,
                    cosmosDbConnectionContent.CollectionId!);
            });

            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            var contentService = serviceProvider.GetService<IDocumentStore>();
            var userSessionDocumentClient = serviceProvider.GetService<Microsoft.Azure.Documents.IDocumentClient>();

            ServicePointManager.DefaultConnectionLimit = 1000;

            var connectionStringDestination = $"AccountEndpoint={cosmosDbConnectionAssessment.EndpointUrl};AccountKey={cosmosDbConnectionAssessment.AccessKey};";

            var destinationDocumentClient = new CosmosClient(
                connectionStringDestination,
                new CosmosClientOptions
                {
                    MaxRequestsPerTcpConnection = 1000,
                    MaxTcpConnectionsPerEndpoint = 1000,
                    ConnectionMode = connectionMode,
                    AllowBulkExecution = true,
                });

            var migrationService = new MigrationService(
                contentService,
                userSessionDocumentClient,
                destinationDocumentClient,
                cosmosDbConnectionAssessment.DatabaseId,
                cosmosDbConnectionAssessment.CollectionId,
                cosmosDbDestinationRUs,
                useBookmark,
                cutoffDateTime,
                runfromerrorfileonly);

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
            var sourceDocumentClient = new Microsoft.Azure.Documents.Client.DocumentClient(
                cosmosDbConnectionLegacyUserSessions.EndpointUrl,
                cosmosDbConnectionLegacyUserSessions.AccessKey,
                new Microsoft.Azure.Documents.Client.ConnectionPolicy
                {
                    MaxConnectionLimit = 1000,
                    ConnectionMode = connectionMode == ConnectionMode.Direct ? Microsoft.Azure.Documents.Client.ConnectionMode.Direct : Microsoft.Azure.Documents.Client.ConnectionMode.Gateway,
                    ConnectionProtocol = Microsoft.Azure.Documents.Client.Protocol.Tcp
                });

            var migrationService = new PopulateTestDataService(sourceDocumentClient, cosmosDbDestinationRUs);

            Activity.Current = new Activity("Dysac test data population").Start();
            await migrationService.Start();
        }
    }
}