using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class AssessmentCompletePage
    {
        private ScenarioContext _scenarioContext;

        public AssessmentCompletePage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement btnSeeResults => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-button.ncs-button__primary.app-button"));

        public void ClickSeeResults()
        {
            btnSeeResults.Click();
        }
    }
}