using DFC.App.DiscoverSkillsCareers.Migration.Services;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;

namespace DFC.App.DiscoverSkillsCareers.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
   .AddEnvironmentVariables()
   .AddCommandLine(args)
   .Build();

            var cosmosDbConnectionContent = Configuration.GetSection("Configuration:CosmosDbConnections:DysacContent").Get<CosmosDbConnection>();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                // Change this
                .AddDocumentServices<DysacQuestionSetContentModel>(cosmosDbConnectionContent, true)
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //do the actual work here
            var questionSetDocumentService = serviceProvider.GetService<IDocumentService<DysacQuestionSetContentModel>>();

            var migrationService = new MigrationService(questionSetDocumentService);
            migrationService.Start();
        }
    }
}
