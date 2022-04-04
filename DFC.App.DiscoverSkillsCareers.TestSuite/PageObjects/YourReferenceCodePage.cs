using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class YourReferenceCodePage
    {
        private ScenarioContext _scenarioContext;
        public YourReferenceCodePage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        public string ReferenceCode { get; set; }

        IWebElement optionReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".app-your-reference__code"));

        IWebElement date => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-grid-column-one-third.govuk-body-s"));

        IWebElement fldPhoneNumber => _scenarioContext.GetWebDriver().FindElement(By.Id("code"));

        IWebElement divPhoneNumberTopValidation => _scenarioContext.GetWebDriver().FindElement(By.Id("dysac-validation-summary"));
        
        IWebElement txtPhoneNumberBottomValidation => _scenarioContext.GetWebDriver().FindElement(By.Id("code-error"));
        IWebElement lnkBack => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Back"));

        public void GetReferenceCode()
        {
            ReferenceCode = optionReferenceCode.Text;
        }

        public string GetDate()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            return date.Text;
        }

        public string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd MMM yyy");
        }

        public void EnterPhoneNumber(string phoneNumber)
        {
            fldPhoneNumber.SendKeys(phoneNumber);
        }

        public bool ValidationBoxPresent()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            return divPhoneNumberTopValidation.Displayed;
        }

        public string PhoneValidationMsg()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            return txtPhoneNumberBottomValidation.Text;
        }

        public void ClickBackLink()
        {
            lnkBack.Click();
        }
    }
}