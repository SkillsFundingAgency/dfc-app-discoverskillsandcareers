using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;
using static DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers.QuestionsAnswers;

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
        IWebElement saveMyProgress => _scenarioContext.GetWebDriver().FindElement(By.XPath("(//a[contains(text(), 'Save my progress')])[2]"));
        IWebElement optionReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='SelectedOption-2']"));

        public string GetYesNoQuestion()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
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

        public void ClickSaveMyProgress()
        {
            saveMyProgress.Click();
        }

        public void ClickGetAReferenceCode()
        {
            optionReferenceCode.Click();
        }

        public List<string> AnswerFilterQuestions(IList<AnswerData> filterQuestionsAndAnswers)
        {
            List<string> questions = new List<string> { };

            for (int i = 0; i < filterQuestionsAndAnswers.Count; i++)
            {
                for (int j = 0; j < filterQuestionsAndAnswers.Count; j++)
                {
                    WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));

                    if (question.Text == filterQuestionsAndAnswers[j].Question)
                    {
                        questions.Add(question.Text);

                        if (filterQuestionsAndAnswers[j].Answer == "Yes")
                        {
                            answerYes.Click();
                        }
                        else if (filterQuestionsAndAnswers[j].Answer == "No")
                        {
                            answerNo.Click();
                        }

                        next.Click();

                        break;
                    }
                }
            }

            return questions;
        }
    }
}