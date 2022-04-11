using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Extensions
{
    public static class DysacScenarioContextExtension
    {
        public static string GetDYSACApiBaseUrl(this ScenarioContext context) { return context.GetEnv().DYSACApiBaseUrl; }
    }
}
