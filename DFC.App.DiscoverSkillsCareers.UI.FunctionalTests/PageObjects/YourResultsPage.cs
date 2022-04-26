using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        IWebElement lnkSeeMatches => _scenarioContext.GetWebDriver().FindElement(By.LinkText("See matches"));
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
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            return Support.GetAllText(_scenarioContext.GetWebDriver(), By.CssSelector(".govuk-list.govuk-list--bullet > li")).Contains(jobCategory);
        }

        public IList<IWebElement> GetTraitsUI()
        {
            return _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".govuk-text > ul:nth-of-type(1) li"));
        }

        public bool AreTheyInSequence(IEnumerable<Traits> traits)
        {
            //expected (data table) Traits
            string[] a = traits.Select(p => p.TraitText).ToArray();
            //actual (UI) Traits
            string[] b = Support.GetAllText(_scenarioContext.GetWebDriver(), By.CssSelector(".govuk-text > ul:nth-of-type(1) li"));

            return IsSequenceEqual(a, b);
        }

        public bool AreJobRolesInSequence(IEnumerable<JobCategoryRoles> traits, string jobCategory)
        {
            //expected (data table) Traits
            string[] a = traits.Select(p => p.JobRoles).ToArray();
            //actual (UI) Traits
            string[] b = Support.GetAllText(_scenarioContext.GetWebDriver(), By.XPath("//*[@class='govuk-grid-column-two-thirds']//h2//a[contains(text(), '" + jobCategory + "')]//..//..//..//following-sibling::ul//a"));

            return IsSequenceEqual(a, b);
        }

        public bool IsSequenceEqual(string[] a, string[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        public bool VerifyTraits(IEnumerable<Traits> traits)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            bool a_and_b_checks = false;

            string[] expectedTraits = traits.Select(p => p.TraitText).ToArray();

            //translate IWebElements into a collection of strings so they can be compared
            IEnumerable<string> actual = GetTraitsUI().Select(i => i.Text);

            //determines, as bool, if items in 1 and 2 are present in the other
            var traitsVerified = expectedTraits.All(d => actual.Contains(d));

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

        public void ClickAnswerMoreQuestionsButton(string noOfMoreQuestions, string jobCategories)
        {
            try
            {
                lnkSeeMatches.Click();
            }
            catch (NoSuchElementException)
            {

            }

            _scenarioContext.GetWebDriver().FindElement(By.XPath("//a[contains(text(), '" + jobCategories + "')]//..//following-sibling::a[contains(text(), 'Answer " + noOfMoreQuestions + " more question')]")).Click();
        }

        public string GetNumberOfRolesInterestedIn(string jobCategory)
        {
            var text = _scenarioContext.GetWebDriver().FindElement(By.XPath("//*[@class='govuk-grid-column-two-thirds']//h2//a[contains(text(), '" + jobCategory + "')]//..//following-sibling::p[1]")).Text;

            if (text.Contains("roles"))
            {
                return text.Replace("roles you might be interested in", string.Empty).Trim();
            }
            else
            {
                return text.Replace("role you might be interested in", string.Empty).Trim();
            }
        }

        public string GetNoCareersMessage(string jobCategory)
        {
            var text = _scenarioContext.GetWebDriver().FindElement(By.XPath("//*[@class='govuk-grid-column-two-thirds']//h2//a[contains(text(), '" + jobCategory + "')]//..//following-sibling::p[2]//span")).Text;
            return text.Trim();
        }

        public IList<IWebElement> GetRolesUI(string jobCategory)
        {
            return _scenarioContext.GetWebDriver().FindElements(By.XPath("//*[@class='govuk-grid-column-two-thirds']//h2//a[contains(text(), '" + jobCategory + "')]//..//..//..//following-sibling::ul//a"));
        }

        public bool VerifyRoles(IEnumerable<JobCategoryRoles> roles, string jobCategory)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));

            try
            {
                do
                {
                    WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.ClassName("govuk-footer"));
                    _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".show-more > a:nth-of-type(1)")).Click();
                    Thread.Sleep(250);
                } while (_scenarioContext.GetWebDriver().FindElement(By.CssSelector(".show-more > a:nth-of-type(1)")).Displayed);
            }
            catch (NoSuchElementException)
            {

            }
            catch (ElementNotInteractableException)
            {

            }

            bool a_and_b_checks = false;

            string[] expectedTraits = roles.Select(p => p.JobRoles).ToArray();

            //translate IWebElements into a collection of strings so they can be compared
            IEnumerable<string> actual = GetRolesUI(jobCategory).Select(i => i.Text);

            //determines, as bool, if items in 1 and 2 are present in the other
            var traitsVerified = expectedTraits.All(d => actual.Contains(d));

            //B - Check.
            int noOfActualElements = GetRolesUI(jobCategory).Count;
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

        public void GoBackAnswering()
        {
            string url = _scenarioContext.GetWebDriver().Url.Split(".uk")[0].Trim() + ".uk";
            _scenarioContext.GetWebDriver().Url = url;
        }

        public void ClickSeeResultsForJobCatergory(string jobCategory)
        {
            try
            {
                _scenarioContext.GetWebDriver().FindElement(By.XPath("(//*[@class='govuk-grid-column-two-thirds']//h2//a[contains(text(), '" + jobCategory + "')]//..//..//..//following-sibling::a)[2]")).Click();
            }
            catch (NoSuchElementException)
            {

            }
        }
    }
}