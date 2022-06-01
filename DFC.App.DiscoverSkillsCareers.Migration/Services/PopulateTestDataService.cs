using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using DFC.App.DiscoverSkillsCareers.Models;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class PopulateTestDataService : IMigrationService
    {
        private readonly IDocumentClient destinationDocumentClient;
        private static Random random = new Random();
        
        public PopulateTestDataService(
            IDocumentClient destinationDocumentClient)
        {
            this.destinationDocumentClient = destinationDocumentClient;
        }

        public async Task Start()
        {
            var itemsToInsert = 300000;
            var batchSize = 230;  // 7 at 400, 20 at 1000, 230 at 10,000, 8000 at 40,000
            var counter = 0;

            while (counter < itemsToInsert)
            {
                var populateTasks = new List<Task>();

                for (int idx = 0, len = batchSize; idx < len; idx++)
                {
                    var assessment = new DysacAssessment
                    {
                        AssessmentCode = RandomString(14),
                        Id = Guid.NewGuid(),
                        PartitionKey = "/Assessment",
                        Questions = GetQuestions()
                    };

                    populateTasks.Add(Add(assessment, counter + idx + 1, itemsToInsert));
                }
                
                await Task.WhenAll(populateTasks);
                counter += batchSize;  
            }
        }
        
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();
        }
        
        private async Task Add(DysacAssessment migratedAssessment, int index, int count)
        {
            Log($"Started creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var resourceResponse = await destinationDocumentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri("dfc-app-dysac", "assessment"),
                migratedAssessment,
                new RequestOptions());
            
            var charge = resourceResponse.RequestCharge;
            
            Log($"Finished creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds. Charge was {charge} RUs");
        }
        
        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        private List<ShortQuestion> GetQuestions()
        {
            return new List<ShortQuestion>
            {
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },

                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },

                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
                new ShortQuestion
                {
                    Id = Guid.NewGuid(),
                    Ordinal = 0,
                    IsNegative = false,
                    QuestionText = RandomString(100),
                },
            };
        }
    }
}