using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.PageObjects
{
    class YesNoQuestionsPage
    {
        private ScenarioContext _scenarioContext;
        public YesNoQuestionsPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        public string ReferenceCode { get; set; }

        IWebElement question => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-fieldset__heading"));
        IWebElement answerYes => _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-1"));
        IWebElement answerNo => _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-2"));
        IWebElement next => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-button.app-button.btn-next-question"));

        public string GetYesNoQuestion()
        {
            return question.Text.Trim();
        }

        public void ClickAnswerRadioButton(string answerOption)
        {
            switch(answerOption)
            {
                case "Yes":
                    answerYes.Click();
                    break;
                case "No":
                    answerNo.Click();
                    break;
            }
        }

        public void ClickNext()
        {
            next.Click();
        }
    }
}