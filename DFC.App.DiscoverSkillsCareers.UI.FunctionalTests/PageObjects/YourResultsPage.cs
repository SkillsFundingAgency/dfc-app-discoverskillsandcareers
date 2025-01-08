using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers;
using Microsoft.Azure.Cosmos.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
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

        IWebElement lnkSeeMatches => _scenarioContext.GetWebDriver().FindElement(By.Id("accordion-default-heading-1"));
        IWebElement btnNext => _scenarioContext.GetWebDriver().FindElement(By.ClassName("btn-next-question"));
        IWebElement lnkBack => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-back-link"));
        By yesRadioButton => By.Id("selected_answer-1");
        By noRadioButton => By.Id("selected_answer-2");

        IWebElement btnNextQuestion => _scenarioContext.GetWebDriver().FindElement(By.Id("dysac-submit-button"));


        public bool VerifyJobCategories(IEnumerable<JobCategories> jobCategories)
        {
           // WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

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

        public bool VerifyAllJobCategories(IEnumerable<JobCategories> jobCategories)
        {
            // WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            bool a_and_b_checks = false;

            string[] allJobCategories = jobCategories.Select(p => p.JobCategory).ToArray();

            IList<IWebElement> jobCategoriesUI;

            try
            {
                jobCategoriesUI = GetAllJobCategories();
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

        public bool VerifyAllJobRoles(IEnumerable<JobRoles> jobRoles)
        {
            // WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.LinkText("Back to top"));

            bool a_and_b_checks = false;

            string[] allJobRoles = jobRoles.Select(p => p.JobRole).ToArray();

            IList<IWebElement> jobRolesUI;

            try
            {
                jobRolesUI = GetAllJobRolesFortheCategory();
            }
            catch (NoSuchElementException)
            {
                jobRolesUI = null;
            }

            //translate IWebElements above into a collection of strings so they can be compared
            IEnumerable<string> actual = jobRolesUI.Select(i => i.Text);

            //determines, as bool, if items in 1 and 2 are present in the other
            var optionsVerified = allJobRoles.All(d => actual.Contains(d));

            //B - Check.
            int noOfActualElements = jobRolesUI.Count;
            bool optionsEqual = false;

            if (allJobRoles.Length == noOfActualElements)
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
            
            IWebElement parentElement = _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[contains(@class,'dysac-job-role-header-content')]/following-sibling::div[contains(@class,'dysac-job-category-tile-container')]"));

            IList<IWebElement> childElements = parentElement.FindElements(By.XPath(".//div[contains(@class,'dysac-job-category-card')]//h3[a[@href]]"));
           

            return childElements;
        }

        public IList<IWebElement> GetAllJobCategories()
        {

            return _scenarioContext.GetWebDriver().FindElements(By.XPath(".//div[contains(@class,'dysac-job-category-card')]//h3[a[@href]]"));
        }

        public IList<IWebElement> GetAllJobRolesFortheCategory()
        {
            return _scenarioContext.GetWebDriver().FindElements(By.XPath(".//div[contains(@class,'ncs-card-basic dysac-job-role-card')]//h3[a[@href]]"));
        }

        public IList<IWebElement> GetRemainingJobCategories()
        {
            IWebElement parent_Element = _scenarioContext.GetWebDriver().FindElement(By.XPath("//div[@class='govuk-accordion__section-content']"));
            IList<IWebElement> child_Elements = parent_Element.FindElements(By.CssSelector(".ncs-card-with-image.dysac-job-category-card"));

            return child_Elements;
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

        public void ClickJobCategory(string jobCategory)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("accordion-default-heading-1"));
            IWebElement category = _scenarioContext.GetWebDriver().FindElement(By.LinkText("jobCategory"));
            category.Click();

        }

        public void AnswerAllQuestions(string answerOption)
        {
            do
            {
                
                    AnswerOptionClick(answerOption);
                    btnNextQuestion.Click();
            }
            while (_scenarioContext.GetWebDriver().FindElements(By.Id("dysac-submit-button")).Count!=0);
        }

        public bool AnswerOptionClick(string answerOption)
        {
            WebDriverExtension.WaitUntilElementFound(_scenarioContext.GetWebDriver(), By.Id("dysac-submit-button"));
            bool radioButtonSelected = false;

            switch (answerOption)
            {
                case "Yes":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-1")).Click();
                    radioButtonSelected = RadioButtonSelected(By.Id("selected_answer-1"));
                    break;
                case "No":
                    _scenarioContext.GetWebDriver().FindElement(By.Id("selected_answer-2")).Click();
                    radioButtonSelected = RadioButtonSelected(By.Id("selected_answer-2"));
                    break;                
            }

            return radioButtonSelected;
        }

        public bool RadioButtonSelected(By element)
        {
            return _scenarioContext.GetWebDriver().FindElement(element).Selected;
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
                    uiElementData = _scenarioContext.GetWebDriver().FindElement(By.XPath("//a[contains(text(), '" + jobCategoryExpected[i] + "')]/following::a")).Text.Replace("Answer", string.Empty).Replace("more questions", string.Empty).Replace("more question", string.Empty).Trim();
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

        public void ClickAnswerButton(string text)
        {
            var buttonElements = _scenarioContext.GetWebDriver().FindElements(By.ClassName("govuk-button"));

            foreach (var buttonElement in buttonElements)
            {
                var buttonText = CleanAndReBuildWebElement(buttonElement).Text;
                if (buttonText.Contains(text))
                {
                    buttonElement.Click();
                    break;
                }
            }
        }

        public void ClickAnswerLink(string text)
        {
            var linkElements = _scenarioContext.GetWebDriver().FindElements(By.ClassName("govuk-link"));

            foreach (var linkElement in linkElements)
            {
                var linkText = CleanAndReBuildWebElement(linkElement).Text;
                if (linkText.Contains(text))
                {
                    linkElement.Click();
                    break;
                }
            }
        }


        public void ClickBackToPreviousQuestion()
        {
            lnkBack.Click();
        }

        public IWebElement CleanAndReBuildWebElement(IWebElement webElement)
        {
            IWebElement webElement2 = null;
            int num = 0;
            try
            {
                do
                {
                    webElement2 = webElement;
                    try
                    {
                        _ = webElement2.Displayed;
                    }
                    catch (StaleElementReferenceException)
                    {
                        webElement2 = null;                        
                        num++;
                        continue;
                    }

                    break;
                }
                while (num < 20);
            }
            catch (NoSuchElementException message)
            {
               
            }

            return webElement2;
        }

        public bool RadioButtonYesIsSelected()
        {
            return RadioButtonSelected(yesRadioButton);
        }

        public void MyLastAnwserIsShown()
        {
            Assert.That(RadioButtonYesIsSelected(), Is.True, "Radio button Yes is not selected");
        }

        public void NextQuestion()
        {
            btnNextQuestion.Click();
        }

        public void NoJobRoles()
        {
          IWebElement noJobRoleDiv = _scenarioContext.GetWebDriver().FindElement(By.XPath(".//div[contains(@class,'dysac-job-role-tile-nojob')]//p/span"));

            Assert.AreEqual("No careers were found that might interest you based on your responses.", noJobRoleDiv.Text);

        }
       
    }
}