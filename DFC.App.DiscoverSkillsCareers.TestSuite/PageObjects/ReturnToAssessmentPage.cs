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

        IWebElement optionReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='SelectedOption-2']"));

        IWebElement lnkReturnToAssessment => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Return to assessment"));

        IWebElement txtHeader => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-fieldset__heading"));

        public void SelectReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='govuk-radios__item']/label[@for='SelectedOption-2']"));
            optionReferenceCode.Click();
        }

        public void ClickReturnToAssessment()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
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
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            return txtHeader.Text;
        }
    }
}