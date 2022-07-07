using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IDocumentStore
    {
        public Task<T?> GetContentByIdAsync<T>(Guid id, string partitionKey)
            where T : class;

        public Task<List<T>> GetAllContentAsync<T>(string partitionKey)
            where T : class;

        public Task<DysacAssessment?> GetAssessmentAsync(string sessionId);

        public Task UpdateAssessmentAsync(DysacAssessment assessment);

        public Task<HttpStatusCode> CreateContentAsync<T>(T content);

        public Task<HttpStatusCode> UpdateContentAsync<T>(T content);

        public Task<bool> DeleteContentAsync<T>(Guid id, string partitionKey)
            where T : class;
    }
}