using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.StepDefinitions
{
    [Binding]
    public class DYSACSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly DysacPage _dysacPage;

        public DYSACSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _dysacPage = new DysacPage(_scenarioContext);
        }

        [Given]
        public void GivenILoadTheDYSACPage()
        {
            _scenarioContext.GetWebDriver().Navigate().GoToUrl(_scenarioContext.GetEnv().DYSACApiBaseUrl);
        }

        [When]
        public void WhenIClickOnAssessment()
        {
            _dysacPage.ClickStartAssessment();
        }

        [Then(@"The first question is displayed; (.*)")]
        public void ThenTheFirstQuestionIsDisplayed(string firstQuestion)
        {
            Assert.Equal(firstQuestion, _dysacPage.GetQuestionText());
        }

        [When(@"I select (.*) option")]
        public void WhenISelectStronglyAgreeOption(string selectedOption)
        {
            _dysacPage.SelectAnswer();
        }

        [When(@"I click Next")]
        public void WhenIClickNext()
        {
            _dysacPage.ClickNextQuestion();
        }

        [Then(@"The next question is displayed; (.*)")]
        public void ThenTheSecondQuestionIsDisplayed(string questionText)
        {
            Assert.Equal(questionText, _dysacPage.GetQuestionText());
        }

        [Then(@"Percentage completion is (.*)")]
        public void ThenPercentageCompletionIs(string percentageComplete)
        {
            Assert.Equal(percentageComplete, _dysacPage.GetPercentageComplete());
        }

        [Then(@"The assessment is complete")]
        public void ThenTheAssessmentIsComplete()
        {
            Assert.Equal("Assessment complete", _dysacPage.GetAssessmentCompleteText());
        }

        [Then(@"The task is (.*) complete")]
        public void ThenTheTaskIsComplete(string hunderedPercent)
        {
            Assert.Equal(hunderedPercent, _dysacPage.GetPercentageCompleteWhenFinished());
        }

        [Then(@"The error is displayed; (.*)")]
        public void ThenTheErrorIsDisplayedChooseAnAnswerToTheStatement(string errorMessage)
        {
            Assert.Equal(errorMessage, _dysacPage.GetChooseAnOptionError());
        }


    }
}
