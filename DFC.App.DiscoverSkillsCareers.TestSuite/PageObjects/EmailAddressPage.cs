using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class EmailAddressPage
    {
        private ScenarioContext _scenarioContext;

        public EmailAddressPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement fldEmailAddress => _scenarioContext.GetWebDriver().FindElement(By.Id("code"));
        IWebElement lnkBack => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Back"));

        public void EnterEmailAddress(string emailAddress)
        {
            fldEmailAddress.SendKeys(emailAddress);
        }

        public void GoFoward()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("dysac-submit-button"));
            _scenarioContext.GetWebDriver().Navigate().Forward();
        }

        public void ClickBack()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("code"));
            lnkBack.Click();
        }
    }
}
