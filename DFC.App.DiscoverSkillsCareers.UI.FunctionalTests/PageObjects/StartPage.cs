using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class StartPage
    {
        private ScenarioContext _scenarioContext;

        public StartPage(ScenarioContext context)
        {
            _scenarioContext = context;

        }

        public StartPage NavigateTo(string sPath)
        {
            string url = _scenarioContext.GetEnv().DYSACApiBaseUrl + (sPath.StartsWith("/") ? string.Empty : "/") + sPath;
            _scenarioContext.GetWebDriver().Url = url;
            return this;
        }
    }
}