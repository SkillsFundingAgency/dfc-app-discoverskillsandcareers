using DFC.App.DiscoverSkillsCareers.Migration.Services;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
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

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                // Change this
                .AddDocumentServices<DysacTraitContentModel>(cosmosDbConnectionContent, true)
                .AddDocumentServices<DysacAssessment>(cosmosDbConnectionContent, true)
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //do the actual work here
            var questionSetDocumentService = serviceProvider.GetService<IDocumentService<DysacTraitContentModel>>();
            var assessmentDocumentService = serviceProvider.GetService<IDocumentService<DysacAssessment>>();

            var migrationService = new MigrationService(questionSetDocumentService, assessmentDocumentService);
            Activity.Current = new Activity("Dysac Assessment Migration").Start();
            await migrationService.Start();
        }
    }
}
