using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects
{
    class YourResultsPage
    {
        private ScenarioContext _scenarioContext;

        public YourResultsPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement txtHeader => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-heading-xl"));
        IWebElement lnkSeeMatches => _scenarioContext.GetWebDriver().FindElement(By.LinkText("See matches"));
        IWebElement btnNext => _scenarioContext.GetWebDriver().FindElement(By.ClassName("btn-next-question"));
        IWebElement txtYourResultStatement => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.govuk-list--bullet > li:nth-of-type(1)"));

        public bool VerifyJobCategories(IEnumerable<JobCategories> jobCategories)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            bool a_and_b_checks = false;

            string[] allJobCategories = jobCategories.Select(p => p.JobCategory).ToArray();

            IList<IWebElement> jobCategoriesUI;

            try
            {
                jobCategoriesUI = GetJobCategories();
            }
            catch (NoSuchElementException)
            {
                jobCategoriesUI = null;
            }

            //translate IWebElements above into a collection of strings so they can be compared
            IEnumerable<string> actual = jobCategoriesUI.Select(i => i.Text);

            //determines, as bool, if items in 1 and 2 are present in the other
            var optionsVerified = allJobCategories.All(d => actual.Contains(d));

            //B - Check.
            int noOfActualElements = jobCategoriesUI.Count;
            bool optionsEqual = false;

            if (allJobCategories.Length == noOfActualElements)
            {
                optionsEqual = true;
            }

            if (optionsVerified == true && optionsEqual == true)
            {
                a_and_b_checks = true;
            }

            return a_and_b_checks;
        }

        public IList<IWebElement> GetJobCategories()
        {
            return _scenarioContext.GetWebDriver().FindElements(By.CssSelector("li[class='app-results__item'][style=''] h3"));
        }

        public void ClickSeeMatches()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            try
            {
                lnkSeeMatches.Click();
            }
            catch (NoSuchElementException)
            {

            }
        }

        public bool VerifyJobsAndNumberOfAnswers(IEnumerable<JobCategories> jobCategoriesAndNumberOfAnswers)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));
            
            int[] numberExpected = jobCategoriesAndNumberOfAnswers.Select(p => p.NumberOfAnswerMoreQuestions).ToArray();
            string[] jobCategoryExpected = jobCategoriesAndNumberOfAnswers.Select(p => p.JobCategory).ToArray();

            bool jobCategoryAndNumberOfAnswersMatch = true;
            string uiElementData = string.Empty;

            for (int i = 0; i < numberExpected.Count(); i++)
            {
                try
                {
                    uiElementData = _scenarioContext.GetWebDriver().FindElement(By.XPath("//a[contains(text(), '" + jobCategoryExpected[i] + "')]//..//following-sibling::a")).Text.Replace("Answer", string.Empty).Replace("more questions", string.Empty).Replace("for " + jobCategoryExpected[i], string.Empty).Trim();
                } 
                catch (NoSuchElementException)
                {
                    jobCategoryAndNumberOfAnswersMatch = false;
                }
                
                if (numberExpected[i].ToString() != uiElementData.Trim())
                {
                    jobCategoryAndNumberOfAnswersMatch = false;
                }
            }

            return jobCategoryAndNumberOfAnswersMatch;
        }

        public void AnswerQuestions(IEnumerable<AnswersShowThat> questionsAndAnswers)
        {
            //string[] PercentProgress = questionsAndAnswers.Select(p => p.PercentProgress).ToArray();
            string[] question = questionsAndAnswers.Select(p => p.Question).ToArray();
            string[] answer = questionsAndAnswers.Select(p => p.Answer).ToArray();

            for (int i = 0; i < question.Count(); i++)
            {
                WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("btn-next-question"));
                _scenarioContext.GetWebDriver().FindElement(By.XPath("//h1[contains(text(), \"" + question[i].Trim() + "\")]//..//following-sibling::div//label[contains(text(), \"" + answer[i].Trim() + "\")]//preceding::input[1]")).Click();
                btnNext.Click();
            }
        }

        public bool GetYourResultStatement(string jobCategory)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            return Support.GetAllText(_scenarioContext.GetWebDriver(), By.CssSelector(".govuk-list.govuk-list--bullet > li")).Contains(jobCategory);
        }
    }
}