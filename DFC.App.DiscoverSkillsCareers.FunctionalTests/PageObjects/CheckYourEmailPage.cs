using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class CheckYourEmailPage
    {
        private ScenarioContext _scenarioContext;

        public CheckYourEmailPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement txtHeader => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-heading-xl"));
        IWebElement txtEmailNotification => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-heading-xl + p"));
        IWebElement lnkBack => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Back"));

        public string GetHeader()
        {
            return txtHeader.Text;
        }

        public string GetEmailAddressNotification()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to start"));
            return txtEmailNotification.Text.Replace("An email has been sent to ", string.Empty).Trim();
        }

        public void ClickBack()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to start"));
            lnkBack.Click();
        }
    }
}