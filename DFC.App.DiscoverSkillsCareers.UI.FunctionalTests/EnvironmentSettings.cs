using System;

using Microsoft.Extensions.Configuration;

namespace DFC.App.DiscoverSkillsCareers.TestSuite
{
    public class EnvironmentSettings
    {
        private static readonly IConfiguration Configuration =
        new EnvironmentSettingsConfigurationBuilder(nameof(TestSuite)).BuildConfiguration();

        public IConfiguration GetConfiguration() { return Configuration; }

        //public string CosmosDbConnectionAccessKey => Configuration["Configuration:CosmosDbConnections:DysacContent:AccessKey"];
        //public string CosmosDbConnectionEndpoint => Configuration["Configuration:CosmosDbConnections:DysacContent:EndpointUrl"];
        //public string CosmosDbConnectionDatabaseId => Configuration["Configuration:CosmosDbConnections:DysacContent:DatabaseId"];
        //public string DysacContentCollectionId => Configuration["Configuration:CosmosDbConnections:DysacContent:CollectionId"];
        //public string DysacAssessmentCollectionId => Configuration["Configuration:CosmosDbConnections:DysacAssessment:CollectionId"];
        //public string SessionStateCollectionId => Configuration["Configuration:CosmosDbConnections:SessionState:CollectionId"];
        //public string DYSACApiBaseUrl => Configuration["DYSACApi:BaseUrl"];
        public string DYSACApiBaseUrl = "https://dev-beta.nationalcareersservice.org.uk/discover-your-skills-and-careers";
        public bool PipelineRun => Environment.GetEnvironmentVariable("SYSTEM_TEAMFOUNDATIONCOLLECTIONURI") == "https://sfa-gov-uk.visualstudio.com/";
    }
}
