using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
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

        public bool VerifyJobCategories(IEnumerable<JobCategories> jobCategories)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            bool a_and_b_checks = false;

            string[] allJobCategories = jobCategories.Select(p => p.JobCategory).ToArray();

            //IList<IWebElement> jobCategoriesUI = _scenarioContext.GetWebDriver().FindElements(By.CssSelector("li[class='app-results__item'][style=''] h3"));
            IList<IWebElement> jobCategoriesUI = GetJobCategories();
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
            lnkSeeMatches.Click(); 
        }

        public bool VerifyJobsAndNumberOfAnswers(IEnumerable<JobCategories> jobCategoriesAndNumberOfAnswers)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));
            
            int[] numberExpected = jobCategoriesAndNumberOfAnswers.Select(p => p.NumberOfAnswerMoreQuestions).ToArray();
            string[] jobCategoryExpected = jobCategoriesAndNumberOfAnswers.Select(p => p.JobCategory).ToArray();

            bool jobCategoryAndNumberOfAnswersMatch = true;
            
            for (int i = 0; i < numberExpected.Count(); i++)
            {
                var uiElementData = _scenarioContext.GetWebDriver().FindElement(By.XPath("//a[contains(text(), '" + jobCategoryExpected[i] + "')]//..//following-sibling::a")).Text.Replace("Answer", string.Empty).Replace("more questions", string.Empty).Replace("for " + jobCategoryExpected[i], string.Empty).Trim();

                if (numberExpected[i].ToString() != uiElementData.Trim())
                {
                    jobCategoryAndNumberOfAnswersMatch = false;
                }
            }

            return jobCategoryAndNumberOfAnswersMatch;
        }
    }
}