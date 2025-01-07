using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class StartPage
    {
        private ScenarioContext _scenarioContext;

        IWebElement btnStartAssessment => _scenarioContext.GetWebDriver().FindElement(By.XPath("//button[contains(text(),'Start your assessment')]"));

        public StartPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        public void ClickStartYourAssessment()
        {            
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath("//button[contains(text(),'Start your assessment')]"));
            btnStartAssessment.Click();

        }

    }
}