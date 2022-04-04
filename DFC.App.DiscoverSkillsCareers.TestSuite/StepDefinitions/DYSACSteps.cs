using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.StepDefinitions
{
    [Binding]
    public class DYSACSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly DysacPage _dysacPage;
        private readonly ReturnToAssessmentPage _returnToAssessmentPage;
        private readonly YourReferenceCodePage _yourReferenceCodePage;
        private readonly CheckYourPhonePage _checkYourPhonePage;
        private string theAnswerOption;
        private string dateOnPage;
        private string _phoneNumber;
        private string _percentCompleted;

        public DYSACSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _dysacPage = new DysacPage(_scenarioContext);
            _returnToAssessmentPage = new ReturnToAssessmentPage(_scenarioContext);
            _yourReferenceCodePage = new YourReferenceCodePage(_scenarioContext);
            _checkYourPhonePage = new CheckYourPhonePage(_scenarioContext);
        }

        [Given]
        public void GivenILoadTheDYSACPage()
        {
            _scenarioContext.GetWebDriver().Navigate().GoToUrl(_scenarioContext.GetEnv().DYSACApiBaseUrl);
        }

        [Given(@"I click on Assessment")]
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

        [When(@"I click Back to previous statement link")]
        public void WhenIClickBackToPreviousStatementLink()
        {
            _dysacPage.ClickPreviousStatement();
        }

        [When(@"I save my progress")]
        public void WhenISaveMyProgress()
        {
            _dysacPage.ClickSaveProgress();
        }

        [When(@"I select reference code to return to the assessment")]
        public void WhenSelectingReferenceCodeToReturnToTheAssessment()
        {
            _dysacPage.SelectReferenceCode();
        }

        [When(@"I click continue")]
        public void WhenIClickContinue()
        {
            _dysacPage.ClickContinueToSaveProgress();
        }

        [Then(@"The reference code is displayed")]
        public void ThenTheReferenceCodeIsDisplayed()
        {
            Assert.NotEmpty(_dysacPage.GetReferenceCode());
        }

        [When(@"I click See results button")]
        public void WhenIClickSeeResultsButton()
        {
            _dysacPage.ClickSeeResults();
        }

        [Then(@"The results are displayed")]
        public void ThenTheResultsAreDisplayed()
        {
            Assert.NotEmpty(_dysacPage.GetResultText());
        }

        [When(@"I select ""(.*)"" option and answer questions to the end")]
        public void WhenISelectOptionAndAnswerQuestionsToTheEnd(string answerOption)
        {
            theAnswerOption = answerOption;
        }

        [Then(@"the questions in turn have the following class attributes")]
        public void ThenTheQuestionsInTurnHaveTheFollowingClassAttributes(Table table)
        {
            IEnumerable<LocatorAttributes> theAttributes = table.CreateSet<LocatorAttributes>().ToList();
            
            Assert.True(_dysacPage.VerifyProgressBar(theAttributes, theAnswerOption), "Progress bar not proceeding steadily");
        }

        [Given(@"I select the ""(.*)"" option")]
        public void GivenISelectTheOption(string answerOption)
        {
            theAnswerOption = answerOption;
        }

        [Given(@"I proceed with answering questions up to ""(.*)"" percent")]
        [Given(@"I proceed with answering questions up to (.*) percent")]
        public void GivenIProceedWithAnsweringQuestionsUpToPercent(string percentComplete)
        {
            _percentCompleted = percentComplete;
            _dysacPage.AnswerQuestions(theAnswerOption, percentComplete);
        }

        [When(@"I save progress")]
        [Given(@"I save progress")]
        public void GivenISaveProgress()
        {
            _dysacPage.ClickSaveProgress();
        }

        [When(@"I choose the ""(.*)"" option of returning to assessment")]
        [Given(@"I choose the ""(.*)"" option of returning to assessment")]
        public void GivenIChooseTheOptionOfReturningToAssessment(string assessmentReturnOption)
        {
            _returnToAssessmentPage.SelectReferenceCode();
            _dysacPage.ClickContinueToSaveProgress();
        }

        [When(@"I make a note of the reference code")]
        [Given(@"I make a note of the reference code")]
        public void GivenIMakeANoteOfTheReferenceCode()
        {
            _yourReferenceCodePage.GetReferenceCode();
        }

        [When(@"I use the reference code to return to my assessment from the Dysac home page")]
        public void WhenIUseTheReferenceCodeToReturnToMyAssessmentFromTheDysacHomePage()
        {
            _dysacPage.EnterYourReference(_yourReferenceCodePage.ReferenceCode);
            _dysacPage.ClickContinueToSaveProgress();
        }

        [Then(@"I am at the question where I left off")]
        public void ThenIAmAtTheQuestionWhereILeftOff()
        {
            NUnit.Framework.Assert.AreEqual(_percentCompleted + "%", _dysacPage.GetPercentageComplete(), "Reference code did not go to the correct percentage completed.");
        }

        [When(@"I view the date on the resultant page")]
        public void WhenIViewTheDateOnTheResultantPage()
        {
            dateOnPage = _yourReferenceCodePage.GetDate();
        }

        [Then(@"the date is todays date")]
        public void ThenTheDateIsTodaysDate()
        {
            NUnit.Framework.Assert.AreEqual(dateOnPage, DateTime.Now.ToString("d MMMM yyy"), "The date is incorrect.");
        }

        [When(@"I click the Return to assessment link")]
        public void WhenIClickTheReturnToAssessmentLink()
        {
            _returnToAssessmentPage.ClickReturnToAssessment();
        }

        [When(@"I supply phone number ""(.*)""")]
        public void WhenISupplyPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            _yourReferenceCodePage.EnterPhoneNumber(phoneNumber);
            _dysacPage.ClickContinueToSaveProgress();
        }

        [Then(@"the phone number appears on the Check your phone page")]
        public void ThenThePhoneNumberAppearsOnTheCheckYourPhonePage()
        {
            NUnit.Framework.Assert.AreEqual(_phoneNumber, _checkYourPhonePage.GetPhoneNumber().Replace("A text message has been sent to", string.Empty).Trim(), "The phone number is incorrect.");
        }

        [When(@"I click the Return to assessment button")]
        public void WhenIClickTheReturnToAssessmentButton()
        {
            _checkYourPhonePage.ReturnToAssessment();
        }

        [When(@"I go Back and I click the Back to start link")]
        public void WhenIGoBackAndIClickTheBackToStartLink()
        {
            _dysacPage.GoBack();
            _checkYourPhonePage.ClickBackToStart();
        }

        [Then(@"I am navigate to the Dysac home page")]
        public void ThenIAmNavigateToTheDysacHomePage()
        {
            NUnit.Framework.Assert.AreEqual(_scenarioContext.GetEnv().DYSACApiBaseUrl, _dysacPage.GetUrl(), "Link did not navigate to the home page");
        }

        [Then(@"the ""(.*)"" radio button is present on the resultant page")]
        public void ThenTheRadioButtonIsPresentOnTheResultantPage(string radioButton)
        {
            NUnit.Framework.Assert.AreEqual("radio", _returnToAssessmentPage.GetElementAttribute(radioButton), "Radio button not present");
        }

        [Then(@"validation messages are displayed")]
        public void ThenValidationMessagesAreDisplayed()
        {
            NUnit.Framework.Assert.IsTrue(_yourReferenceCodePage.ValidationBoxPresent(), "The validation box is not present.");
            NUnit.Framework.Assert.AreEqual("Enter a phone number", _yourReferenceCodePage.PhoneValidationMsg(), "The validation message was not displayed.");
        }

        [When(@"I click Back")]
        public void WhenIClickBack()
        {
            _yourReferenceCodePage.ClickBackLink();
        }

        [Then(@"I am navigated to the ""(.*)"" page")]
        public void ThenIAmNavigatedToThePage(string pageText)
        {
            NUnit.Framework.Assert.AreEqual(pageText, _returnToAssessmentPage.GetHeaderText(pageText), "Navigation has not been to the correct page.");
        }
    }
}