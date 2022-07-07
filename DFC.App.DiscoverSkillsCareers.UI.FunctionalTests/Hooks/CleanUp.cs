using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Hooks
{
    [Binding]
    public sealed class CleanUp
    {

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public CleanUp(ScenarioContext context, FeatureContext fContext)
        {
            _scenarioContext = context;
            _featureContext = fContext;
        }


        [AfterScenario("webtest", Order = 10)]
        public void TearDownData()
        {
            if (_scenarioContext.ContainsKey("prefix"))
            {
                string prefix = _scenarioContext["prefix"].ToString();
                string prefixField = _scenarioContext["prefixField"].ToString();

                // TODO : Do any tear down here
            }
        }

        [AfterFeature("webtest", Order = 20)]
        public static void CloseWebDriver()
        {
            WebDriverContainer.Instance.CloseDriver();
        }
    }
}
