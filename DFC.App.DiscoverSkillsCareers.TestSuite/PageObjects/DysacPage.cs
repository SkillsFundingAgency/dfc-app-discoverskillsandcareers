using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.TestSuite.StepDefinitions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class DysacPage
    {
        private ScenarioContext _scenarioContext;
        private YourReferenceCodePage _yourReferenceCodePage;
        public DysacPage(ScenarioContext context)
        {
            _scenarioContext = context;
            _yourReferenceCodePage = new YourReferenceCodePage(context);
        }

        public string InitialPercentComplete { get; set; }

        IWebElement btnStartAssessment => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-button--start"));
        IWebElement question => _scenarioContext.GetWebDriver().FindElement(By.Id("question-heading"));
        IWebElement answerOption => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-radios__label"));
        IWebElement btnNextQuestion => _scenarioContext.GetWebDriver().FindElement(By.ClassName("btn-next-question"));
        IWebElement percentageComplete => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='ncs-progress']/span[@class='ncs-progress__count govuk-body']/strong[1]"));
        IWebElement hunderedpercentComplete => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='ncs-progress__count govuk-body']/span[1]"));
        IWebElement assessmentComplete => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-panel__title"));
        IWebElement chooseAnAnswerError => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-error-summary__body']/ul[@class='govuk-list govuk-error-summary__list']/li/li/a[@class='govuk-link govuk-link--no-visited-state']"));
        IWebElement lnkPreviousStatement => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-back-link"));
        IWebElement linkSaveProgress => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='app-sidebar app-save-panel app-save-panel--alt']/p/a[@class='govuk-link govuk-link--no-visited-state']"));
        IWebElement optionReferenceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-radios__item']/label[@for='SelectedOption-2']"));
        IWebElement btnContinueSaveProgress => _scenarioContext.GetWebDriver().FindElement(By.Id("dysac-submit-button"));
        IWebElement referenceCode => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='app-your-reference govuk-body']/p[1]/span[1]"));
        IWebElement btnSeeResults => _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[@class='govuk-grid-column-two-thirds'][1]/a[@class='govuk-button app-button']"));
        IWebElement results => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".app-results h2.govuk-heading-l"));
        IWebElement answer => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".app-results h2.govuk-heading-l"));
        IWebElement enterYourReference => _scenarioContext.GetWebDriver().FindElement(By.Id("code"));

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
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='govuk-error-summary__body']/ul[@class='govuk-list govuk-error-summary__list']/li/li/a[@class='govuk-link govuk-link--no-visited-state']"));
            return chooseAnAnswerError.GetElementText();
        }

        public void ClickPreviousStatement()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.ClassName("govuk-back-link"));
            lnkPreviousStatement.Click();
        }

        public void ClickSaveProgress()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='app-sidebar app-save-panel app-save-panel--alt']/p/a[@class='govuk-link govuk-link--no-visited-state']"));
            InitialPercentComplete = GetPercentageComplete();
            linkSaveProgress.Click();
        }

        public void SelectReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='govuk-radios__item']/label[@for='SelectedOption-2']"));
            optionReferenceCode.Click();
        }

        public void ClickContinueToSaveProgress()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.Id("dysac-submit-button"));
            btnContinueSaveProgress.Click();
        }

        public string GetReferenceCode()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='app-your-reference govuk-body']/p[1]/span[1]"));
            return referenceCode.GetElementText();
        }

        public void ClickSeeResults()
        {
            WebDriverExtension.WaitElementToBeClickable(_scenarioContext.GetWebDriver(), By.XPath(".//div[@class='govuk-grid-column-two-thirds'][1]/a[@class='govuk-button app-button']"));
            btnSeeResults.Click();
        }

        public string GetResultText()
        {
                WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.CssSelector(".app-results h2.govuk-heading-l"));
                return results.GetElementText();
        }

        public void AnswerOptionClick(string answerOption)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("dysac-submit-button"));

            switch (answerOption)
            {
                case "Strongly agree":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-1")).Click();
                    break;
                case "Agree":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-2")).Click();
                    break;
                case "It depends":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-3")).Click();
                    break;
                case "Disagree":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-4")).Click();
                    break;
                case "Strongly disagree":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-5")).Click();
                    break;
            }
        }

        public bool VerifyProgressBar(IEnumerable<LocatorAttributes> attributes, string answerOption)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("dysac-submit-button"));

            string[] innerAttributes = attributes.Select(p => p.ClassAttribute).ToArray();
            bool progressSteady = true;

            for (int i = 0; i < innerAttributes.Length; i++)
            {
                var barProgress = _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".ncs-progress__bar span")).GetAttribute("class");

                if (innerAttributes[i] != barProgress)
                {
                    progressSteady = false;
                    break;
                }

                AnswerOptionClick(answerOption);
                btnNextQuestion.Click();
            }

            return progressSteady;
        }

        public void AnswerQuestions(string answerOption, string percentComplete)
        {
            do
            {
                AnswerOptionClick(answerOption);
                btnNextQuestion.Click();
            }
            while (percentageComplete.Text.Replace("%", string.Empty) != percentComplete);
        }

        public void ReturnToAssessmentOption(string option)
        {
            _scenarioContext.GetWebDriver().FindElement(By.XPath("//*[@class='govuk - radios__item']/label[contains(text(), '" + option + "')]")).Click();
        }

        public void EnterYourReference(string referenceCode)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("code"));
            

            if (_scenarioContext.GetEnv().DYSACApiBaseUrl.Substring(8, 3) == "dev")
            {
                _scenarioContext.GetWebDriver().Url = "https://dev-beta.nationalcareersservice.org.uk/discover-your-skills-and-careers";
            }
            else if (_scenarioContext.GetEnv().DYSACApiBaseUrl.Substring(8, 3) == "sit")
            {
                _scenarioContext.GetWebDriver().Url = "https://sit-beta.nationalcareersservice.org.uk/discover-your-skills-and-careers";
            }

            enterYourReference.SendKeys(referenceCode);
        }

        public void GoBack()
        {
            _scenarioContext.GetWebDriver().Navigate().Back();
        }

        public string GetUrl()
        {
            return _scenarioContext.GetWebDriver().Url.ToString();
        }
    }
}