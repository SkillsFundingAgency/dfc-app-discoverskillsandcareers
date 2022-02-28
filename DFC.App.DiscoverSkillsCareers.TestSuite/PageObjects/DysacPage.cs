using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class DysacPage
    {
        private ScenarioContext _scenarioContext;
        public DysacPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement btnStartAssessment => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-button--start"));
        IWebElement question => _scenarioContext.GetWebDriver().FindElement(By.Id("question-heading"));
        IWebElement answerOption => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-radios__label"));
        IWebElement btnNextQuestion => _scenarioContext.GetWebDriver().FindElement(By.ClassName("btn-next-question"));
        IWebElement percentageComplete => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='ncs-progress']/span[@class='ncs-progress__count govuk-body']/strong[1]"));
        IWebElement hunderedpercentComplete => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='ncs-progress__count govuk-body']/span[1]"));
        IWebElement assessmentComplete => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-panel__title"));
        IWebElement chooseAnAnswerError => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-link--no-visited-state"));

        public void ClickStartAssessment()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-button--start"));
            btnStartAssessment.Click();
        }

        public string GetQuestionText()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.Id("question-heading"));
            return question.GetElementText();
        }

        public void SelectAnswer()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-radios__label"));
            answerOption.Click();
        }

        public void ClickNextQuestion()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("btn-next-question"));
            btnNextQuestion.Click();
        }

        public string GetPercentageComplete()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='ncs-progress']/span[@class='ncs-progress__count govuk-body']/strong[1]"));
            return percentageComplete.GetElementText();
        }

        public string GetPercentageCompleteWhenFinished()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='ncs-progress__count govuk-body']/span[1]"));
            return hunderedpercentComplete.GetElementText();
        }

        public string GetAssessmentCompleteText()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-panel__title"));
            return assessmentComplete.GetElementText();
        }

        public string GetChooseAnOptionError()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-link--no-visited-state"));
            return chooseAnAnswerError.GetElementText();
        }
    }
}
