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

        IWebElement optionReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='contact-2']"));
        IWebElement date => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-grid-column-one-third.govuk-body-s"));
        IWebElement fldPhoneNumber => _scenarioContext.GetWebDriver().FindElement(By.Id("phoneNumber"));
        IWebElement divPhoneNumberTopValidation => _scenarioContext.GetWebDriver().FindElement(By.Id("dysac-validation-summary"));
        IWebElement txtPhoneNumberBottomValidation => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".field-validation-error"));
        IWebElement lnkBack => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Back"));
        IWebElement btnReturnToAssessment => _scenarioContext.GetWebDriver().FindElement(By.XPath("//button[contains(text(),'Return to your assessment')]"));
        IWebElement btnSendRefereneceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath("//button[@id='dysac-start-submit-button']"));

        IWebElement lnkHome => _scenarioContext.GetWebDriver().FindElement(By.LinkText("Skills assessment"));

        IWebElement divReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".app-your-reference__code"));

        IWebElement fldEmailAddress => _scenarioContext.GetWebDriver().FindElement(By.XPath("//input[@name='Email']"));

        IWebElement optionEmailCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='contact']"));

        public void GetReferenceCode()
        {
            ReferenceCode = divReferenceCode.Text;
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
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.Id("dysac-validation-summary"));
            return divPhoneNumberTopValidation.Displayed;
        }

        public string PhoneValidationMsg()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.Id("dysac-validation-summary"));
            return txtPhoneNumberBottomValidation.Text;
        }

        public void ClickReturnToAssessment()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath("//button[contains(text(),'Return to your assessment')]"));
            btnReturnToAssessment.Click();
        }

        public void ClickToSendReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath("//button[@id='dysac-start-submit-button']"));
            btnSendRefereneceCode.Click();
        }

        public void SelectReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            optionReferenceCode.Click();
        }
        public void SelectEmailReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            optionEmailCode.Click();
        }

        public void ReturnToAssessment()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath("//button[contains(text(),'Return to your assessment')]"));
            btnReturnToAssessment.Click();
        }

        public void NavigateToHome()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
            lnkHome.Click();
        }

        public void EnterEmailAddress(string emailAddress)
        {
            fldEmailAddress.SendKeys(emailAddress);
        }

        public void ClickBackLink()
        {
            lnkBack.Click();
        }
    }
}