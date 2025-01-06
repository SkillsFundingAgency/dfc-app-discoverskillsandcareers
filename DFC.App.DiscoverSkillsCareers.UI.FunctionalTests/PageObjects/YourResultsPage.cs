using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections;
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

        IWebElement lnkSeeMatches => _scenarioContext.GetWebDriver().FindElement(By.Id("accordion-default-heading-1"));
        IWebElement btnNext => _scenarioContext.GetWebDriver().FindElement(By.ClassName("btn-next-question"));

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
            return _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".ncs-card-with-image.dysac-job-category-card"));
        }

        public IList<IWebElement> GetRemainingJobCategories()
        {
            return _scenarioContext.GetWebDriver().FindElements(By.XPath("//div[@class='govuk-accordion__section-content']/div[@class='.ncs-card-with-image.dysac-job-category-card']"));
        }



        public void ClickSeeMatches()
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("accordion-default-heading-1"));

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
           // WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            int[] numberExpected = jobCategoriesAndNumberOfAnswers.Select(p => p.NumberOfAnswerMoreQuestions).ToArray();
            string[] jobCategoryExpected = jobCategoriesAndNumberOfAnswers.Select(p => p.JobCategory).ToArray();

            bool jobCategoryAndNumberOfAnswersMatch = true;
            string uiElementData = string.Empty;

            for (int i = 0; i < numberExpected.Count(); i++)
            {
                try
                {
                    uiElementData = _scenarioContext.GetWebDriver().FindElement(By.XPath("//a[contains(text(), '" + jobCategoryExpected[i] + "')]/following::a")).Text.Replace("Answer", string.Empty).Replace("more questions", string.Empty).Trim();
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
            string[] question = questionsAndAnswers.Select(p => p.Question).ToArray();
            string[] answer = questionsAndAnswers.Select(p => p.Answer).ToArray();

            for (int i = 0; i < question.Count(); i++)
            {
                WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("btn-next-question"));
                _scenarioContext.GetWebDriver().FindElement(By.XPath("//h1[contains(text(), \"" + question[i].Trim() + "\")]//..//following-sibling::div//label[contains(text(), \"" + answer[i].Trim() + "\")]//preceding::input[1]")).Click();
                btnNext.Click();
            }
        }

        public static List<string> ExpectedTraits(string traits)
        {
            List<string> expectedTraits = new List<string>();
            expectedTraits.Add(traits);

            return expectedTraits;
        }

        public bool GetYourResultStatement(string jobCategory)
        {
            //WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            return Support.GetAllText(_scenarioContext.GetWebDriver(), By.CssSelector(".govuk-list.govuk-list--bullet > li")).Contains(jobCategory);
        }

        public List<string> GetTraitsUI()
        {
            //return _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".govuk-list.govuk-list--bullet > li"));
            var parentElement = _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".content.dysac-personality-tile-desc"));
            return parentElement.Select(t => t.FindElement(By.TagName("p")).Text.ToLower()).ToList();
        }
        
        public bool VerifyTraits(IEnumerable<Traits> traits)
        {
           // WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            bool a_and_b_checks = false;

            string[] expectedTraits = traits.Select(p => p.TraitText).ToArray();

            //translate IWebElements into a collection of strings so they can be compared
            IEnumerable<string> actual = GetTraitsUI();

            //determines, as bool, if items in 1 and 2 are present in the other
            var traitsVerified = expectedTraits.All(d => actual.Contains(d.ToLower()));

            //B - Check.
            int noOfActualElements = GetTraitsUI().Count;
            bool optionsEqual = false;

            if (expectedTraits.Length == noOfActualElements)
            {
                optionsEqual = true;
            }

            if (traitsVerified == true && optionsEqual == true)
            {
                a_and_b_checks = true;
            }

            return a_and_b_checks;
        }
    }
}