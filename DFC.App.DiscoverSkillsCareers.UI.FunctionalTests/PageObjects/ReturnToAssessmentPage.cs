using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class ReturnToAssessmentPage
    {
        private ScenarioContext _scenarioContext;
        public ReturnToAssessmentPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement optionReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='contact-2']"));
        IWebElement lnkReturnToAssessment => _scenarioContext.GetWebDriver().FindElement(By.XPath("//button[@class='govuk-button ncs-button__primary']"));
        IWebElement txtHeader => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-fieldset__heading"));
        IWebElement optionSendMeEmailLink => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='contact']"));

        public void SelectReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.LinkText("Return to assessment"));
            optionReferenceCode.Click();
        }
        public void SelectSendMeEmailLink()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.LinkText("Return to assessment"));
            optionSendMeEmailLink.Click();
        }

        public void ClickReturnToAssessment()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath("//button[@class='govuk-button ncs-button__primary']"));
            lnkReturnToAssessment.Click();
        }

        public string GetElementAttribute(string elementLabel)
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));

            var attributeValue = _scenarioContext.GetWebDriver().FindElement(By.XPath("//label[contains(text(), '" + elementLabel + "')]//preceding::input")).GetAttribute("type");
            return attributeValue;
        }

        public string GetHeaderText(string text)
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.LinkText("Return to assessment"));
            return txtHeader.Text.Trim();
        }
    }
}