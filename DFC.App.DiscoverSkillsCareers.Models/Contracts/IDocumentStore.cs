using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models.Contracts
{
    public interface IDocumentStore
    {
        public Task<T?> GetContentByIdAsync<T>(Guid id, string partitionKey, [CallerMemberName] string callerMemberName = "")
            where T : class;

        public Task<List<T>> GetAllContentAsync<T>(string partitionKey, [CallerMemberName] string callerMemberName = "")
            where T : class;

        public Task<DysacAssessment?> GetAssessmentAsync(string sessionId, [CallerMemberName] string callerMemberName = "");

        public Task UpdateAssessmentAsync(DysacAssessment assessment, [CallerMemberName] string callerMemberName = "");

        public Task<HttpStatusCode> CreateContentAsync<T>(T content, [CallerMemberName] string callerMemberName = "");

        public Task<HttpStatusCode> UpdateContentAsync<T>(T content, [CallerMemberName] string callerMemberName = "");

        public Task<bool> DeleteContentAsync<T>(Guid id, string partitionKey, [CallerMemberName] string callerMemberName = "")
            where T : class;
    }
}