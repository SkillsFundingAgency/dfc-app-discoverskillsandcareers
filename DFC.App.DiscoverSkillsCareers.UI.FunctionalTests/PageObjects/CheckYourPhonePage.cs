using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class CheckYourPhonePage
    {
        private ScenarioContext _scenarioContext;

        public CheckYourPhonePage(ScenarioContext context)
        {
            _scenarioContext = context;

        }

        IWebElement btnReturnToAssessment => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-main-wrapper .govuk-button.ncs-button__primary"));
        IWebElement txtPhoneNumber => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-main-wrapper p:nth-of-type(1)"));
        IWebElement lnkBackToStart => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Back to start"));

        public string GetPhoneNumber()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            return txtPhoneNumber.Text;
        }

        public void ReturnToAssessment()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            btnReturnToAssessment.Click();
        }

        public void ClickBackToStart()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            lnkBackToStart.Click();
        }
    }
}