using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers;
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
        private readonly EmailAddressPage _emailAddressPage;
        private readonly CheckYourEmailPage _checkYourEmailPage; 
        private readonly YourResultsPage _yourResultsPage; 
        private readonly AssessmentCompletePage _assessmentCompletePage;
        private readonly StartPage _startPage;
        private string _theAnswerOption;
        private string dateOnPage;
        private string _phoneNumber;
        private string _percentCompleted;
        private string _theEmailAddress;
        IEnumerable<Traits> expectedTraits;

        public DYSACSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _dysacPage = new DysacPage(_scenarioContext);
            _returnToAssessmentPage = new ReturnToAssessmentPage(_scenarioContext);
            _yourReferenceCodePage = new YourReferenceCodePage(_scenarioContext);
            _checkYourPhonePage = new CheckYourPhonePage(_scenarioContext);
            _emailAddressPage = new EmailAddressPage(_scenarioContext);
            _checkYourEmailPage = new CheckYourEmailPage(_scenarioContext);
            _yourResultsPage = new YourResultsPage(_scenarioContext);
            _assessmentCompletePage = new AssessmentCompletePage(_scenarioContext);
            _startPage = new StartPage(_scenarioContext);
        }

        [Given]
        public void GivenILoadTheDYSACPage()
        {
            _scenarioContext.GetWebDriver().Navigate().GoToUrl(_scenarioContext.GetEnv().DYSACApiBaseUrl);
        }

        [Given(@"I click on start skills Assessment")]
        [When]
        public void WhenIClickOnStartSkillsAssessment()
        {
            _dysacPage.ClickStartAssessment();
        }
       

        [Given(@"I click on Assessment")]
        [When]
        public void WhenIClickOnAssessment()
        {
            _startPage.ClickStartAssessment();
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
            _dysacPage.ClickGetYourReferenceCode();
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

        [Given(@"I click See results button")]
        [When(@"I click See results button")]
        public void WhenIClickSeeResultsButton()
        {
            _dysacPage.ClickSeeResults();
        }

        [Then(@"the job categories suggestions are (.*) in number")]
        public void ThenTheJobCategoriesSuggestionsAreInNumber(int numberOfJobCategories)
        {
            NUnit.Framework.Assert.AreEqual(numberOfJobCategories, _yourResultsPage.GetJobCategories().Count, "Number of job category suggestions not correct.");
        }

        [Then(@"The results are displayed")]
        public void ThenTheResultsAreDisplayed()
        {
            Assert.NotEmpty(_dysacPage.GetResultText());
        }

        [Given(@"I select ""(.*)"" option and answer questions to the end")]
        [When(@"I select ""(.*)"" option and answer questions to the end")]
        public void WhenISelectOptionAndAnswerQuestionsToTheEnd(string answerOption)
        {
            _theAnswerOption = answerOption;
        }

        [Then(@"the questions in turn have the following class attributes")]
        public void ThenTheQuestionsInTurnHaveTheFollowingClassAttributes(Table table)
        {
            IEnumerable<LocatorAttributes> theAttributes = table.CreateSet<LocatorAttributes>().ToList();
            
            Assert.True(_dysacPage.VerifyProgressBar(theAttributes, _theAnswerOption), "Progress bar not proceeding steadily");
        }

        [Given(@"I select the ""(.*)"" option")]
        public void GivenISelectTheOption(string answerOption)
        {
            _theAnswerOption = answerOption;
        }

        [Given(@"I proceed with answering questions up to ""(.*)"" percent")]
        [Given(@"I proceed with answering questions up to (.*) percent")]
        public void GivenIProceedWithAnsweringQuestionsUpToPercent(string percentComplete)
        {
            _percentCompleted = percentComplete;
            _dysacPage.AnswerQuestions(_theAnswerOption, percentComplete);
        }

        [When(@"I get reference code")]
        [Given(@"I get reference code")]
        public void GivenISaveProgress()
        {
            _dysacPage.ClickGetYourReferenceCode();
        }

        [When(@"I choose the ""(.*)"" option of returning to assessment")]
        [Given(@"I choose the ""(.*)"" option of returning to assessment")]
        public void GivenIChooseTheOptionOfReturningToAssessment(string assessmentReturnOption)
        {
            switch (assessmentReturnOption)
            {
                case "Get a reference code":
                    _returnToAssessmentPage.SelectReferenceCode();
                    _dysacPage.ClickContinueToSaveProgress();
                    break;
                case "Send me an email with a link":
                    _returnToAssessmentPage.SelectSendMeEmailLink();
                    _dysacPage.ClickContinueToSaveProgress();
                    break;
            }
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

        [Then(@"validation messages are displayed for the ""(.*)"" field")]
        public void ThenValidationMessagesAreDisplayedForTheField(string fieldForValidation)
        {
            switch (fieldForValidation)
            {
                case "phone number":
                    NUnit.Framework.Assert.IsTrue(_yourReferenceCodePage.ValidationBoxPresent(), "The validation box is not present.");
                    NUnit.Framework.Assert.AreEqual("Enter a phone number", _yourReferenceCodePage.PhoneValidationMsg(), "The validation message was not displayed.");
                    break;
                case "reference code":
                    NUnit.Framework.Assert.IsTrue(_yourReferenceCodePage.ValidationBoxPresent(), "The validation box is not present.");
                    NUnit.Framework.Assert.AreEqual("Enter your reference", _yourReferenceCodePage.PhoneValidationMsg(), "The validation message was not displayed.");
                    break;
            }
        }

        [When(@"I click Back")]
        public void WhenIClickBack()
        {
            _yourReferenceCodePage.ClickBackLink();
        }

        [Then(@"I am navigated to the ""(.*)"" page")]
        public void ThenIAmNavigatedToThePage(string pageText)
        {
            switch (pageText)
            {
                case "How would you like to return to your assessment?":
                    NUnit.Framework.Assert.AreEqual(pageText, _returnToAssessmentPage.GetHeaderText(pageText), "Navigation has not been to the correct page.");
                    break;
                case "Check your email":
                    NUnit.Framework.Assert.AreEqual(pageText, _checkYourEmailPage.GetHeader(), "Navigation has not been to the correct page.");
                    break;
            }
        }

        [Given(@"I click continue without providing a reference")]
        public void GivenIClickContinueWithoutProvidingAReference()
        {
            _dysacPage.ClickContinueToSaveProgress();
        }

        [When(@"I click Send on the resultant page without providing an email address")]
        public void GivenIClickSendOnTheResultantPageWithoutProvidingAnEmailAddress()
        {
            _dysacPage.ClickContinueToSaveProgress();
        }

        [When(@"I provide email address ""(.*)""")]
        public void WhenIProvideEmailAddress(string emailAddress)
        {
            _theEmailAddress = emailAddress;
            _emailAddressPage.EnterEmailAddress(emailAddress);
            _dysacPage.ClickContinueToSaveProgress();
        }

        [Then(@"the email address used is present in the text on the page")]
        public void ThenTheEmailAddressUsedIsPresentInTheTextOnThePage()
        {
            NUnit.Framework.Assert.AreEqual(_theEmailAddress, _checkYourEmailPage.GetEmailAddressNotification(), "The email address is incorrect");
        }

        [When(@"I click the Back link")]
        public void WhenIClickTheBackLink()
        {
            _checkYourEmailPage.ClickBack();
        }

        [When(@"I click the Back link from the ""(.*)"" page")]
        public void WhenIClickTheBackLinkFromThePage(string page)
        {
            switch (page)
            {
                case "Email address":
                    _emailAddressPage.ClickBack();
                    break;
                case "Check your email":
                    _checkYourEmailPage.ClickBack();
                    break;
            }
        }

        [Then(@"I go forward")]
        public void ThenIGoForward()
        {
            _emailAddressPage.GoFoward();
        }

        [Then(@"I am able to select the ""(.*)"" option for the ""(.*)"" question")]
        public void ThenIAmAbleToSelectTheOptionForTheQuestion(string questionOption, string questionNumber)
        {
            NUnit.Framework.Assert.True(_dysacPage.AnswerOptionClick(questionOption), "Answer option radio button not clicked");
        }

        [Then(@"the initial job categories dispalyed are")]
        public void ThenTheInitialJobCategoriesDispalyedAre(Table table)
        {
            IEnumerable<JobCategories> initialJobCategories = table.CreateSet<JobCategories>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyJobCategories(initialJobCategories), "'3 job categories that might suit you' list is incorrect");
        }

        [Given(@"I answer all questions selecting the (.*) option")]
        public void GivenIAnswerAllQuestionsSelectingTheOption(string questionOption)
        {
            _dysacPage.AnswerAllQuestions(questionOption);
        }

        [When(@"I click See matches to See 7 other career areas that might interest you")]
        public void WhenIClickSeeMatchesToSee7OtherCareerAreasThatMightInterestYou()
        {
            _yourResultsPage.ClickSeeMatches();
        }

        [Then(@"all the job categories dispalyed are")]
        public void ThenAllTheJobCategoriesDispalyedAre(Table table)
        {
            IEnumerable<JobCategories> initialJobCategories = table.CreateSet<JobCategories>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyJobCategories(initialJobCategories), "'10 job categories that might suit you' list is incorrect");
        }

        [Then(@"the following are the job categories suggested and their number of answer more questions")]
        public void ThenTheFollowingAreTheJobCategoriesSuggestedAndTheirNumberOfAnswerMoreQuestions(Table table)
        {
            IEnumerable<JobCategories> jobCategoriesAndNumbers = table.CreateSet<JobCategories>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyJobsAndNumberOfAnswers(jobCategoriesAndNumbers), "The number of answer * more questions for the job categories are not correct.");
        }

        [Given(@"I provide the following answers to the resultant questions")]
        public void GivenIProvideTheFollowingAnswersToTheResultantQuestions(Table table)
        {
            IEnumerable<AnswersShowThat> answers = table.CreateSet<AnswersShowThat>().ToList();
            _yourResultsPage.AnswerQuestions(answers);
            _assessmentCompletePage.ClickSeeResults();
        }

        [Then(@"the Your results page What you told us section displays the trait text ""(.*)""")]
        public void ThenTheYourResultsPageWhatYouToldUsSectionDisplaysTheTraitText(string trait)
        {
            _yourResultsPage.GetYourResultStatement(trait);
        }

        [Then(@"the What you told us section of the Your results page displays the following traits")]
        public void ThenTheWhatYouToldUsSectionOfTheYourResultsPageDisplaysTheFollowingTraits(Table table)
        {
            expectedTraits = table.CreateSet<Traits>().ToList();

            switch (_scenarioContext.ScenarioInfo.Title)
            {
                case "TC23 - Real user interaction 1":
                    _yourResultsPage.VerifyTraits(expectedTraits);
                    break;
                case "TC24 - Real user interaction 2":
                    _yourResultsPage.VerifyTraits(expectedTraits);
                    break;
            }

            NUnit.Framework.Assert.True(_yourResultsPage.VerifyTraits(expectedTraits), "What you told us trait(s) incorrect");
        }

        [Then(@"the following job categories with their corresponding number of answer more questions are displayed")]
        public void ThenTheFollowingJobCategoriesWithTheirCorrespondingNumberOfAnswerMoreQuestionsAreDisplayed(Table table)
        {
            _yourResultsPage.ClickSeeMatches();
            IEnumerable<JobCategories> jobCategories = table.CreateSet<JobCategories>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyJobsAndNumberOfAnswers(jobCategories), "Job categories and or number for answer * more questions incorrect.");
        }
    }
}