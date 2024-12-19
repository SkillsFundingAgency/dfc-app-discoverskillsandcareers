using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class StartPage
    {
        private ScenarioContext _scenarioContext;

        IWebElement btnStartAssessment => _scenarioContext.GetWebDriver().FindElement(By.XPath("//button[@class='govuk-button ncs-button__primary']"));

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

        public void ClickStartAssessment()
        {
            WebDriverExtension.CloseBanner(_scenarioContext.GetWebDriver());
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath("//button[@class='govuk-button ncs-button__primary']"));
            btnStartAssessment.Click();

        }

    }
}