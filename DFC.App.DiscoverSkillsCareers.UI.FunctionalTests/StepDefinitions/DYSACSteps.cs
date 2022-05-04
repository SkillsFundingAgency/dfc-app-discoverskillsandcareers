using DFC.App.DiscoverSkillsCareers.TestSuite.Extensions;
using DFC.App.DiscoverSkillsCareers.TestSuite.Helpers;
using DFC.App.DiscoverSkillsCareers.TestSuite.PageObjects;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers;
using DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.PageObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;
using static DFC.App.DiscoverSkillsCareers.UI.FunctionalTests.Helpers.QuestionsAnswers;

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
        private readonly YesNoQuestionsPage _yesNoQuestionsPage;
        private string _theAnswerOption;
        private string dateOnPage;
        private string _phoneNumber;
        private string _percentCompleted;
        private string _theEmailAddress;
        private string _answerMoreJobCategory;
        private string _currentQuestion;
        private string _currentPage;
        private string filePath = Directory.GetParent(@"../../../").FullName + Path.DirectorySeparatorChar + "Helpers" + "\\";
        IEnumerable<AnswersShowThat> _answers;
        IEnumerable<Traits> _expectedTraits;
        IEnumerable<JobCategoryRoles> _expectedJobRoles;
        String[] _jobCategoryCategoriesInitial;
        IList<IWebElement> _jobCategoryCategoriesSubsequent;

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
            _yesNoQuestionsPage = new YesNoQuestionsPage(_scenarioContext);
        }

        [Given]
        public void GivenILoadTheDYSACPage()
        {
            _scenarioContext.GetWebDriver().Navigate().GoToUrl(_scenarioContext.GetEnv().DYSACApiBaseUrl);
            _dysacPage.AcceptAllCookies();
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

        [Given(@"I select ""(.*)"" answer and proceed")]
        [Given(@"I select ""(.*)"" answer and proceed to the next question")]
        [When(@"I select ""(.*)"" answer and proceed")]
        [When(@"I select ""(.*)"" answer and proceed to the next question")]
        public void WhenISelectAnswerAndProceedToTheNextQuestion(string answerOption)
        {
            _yesNoQuestionsPage.ClickAnswerRadioButton(answerOption);
            _yesNoQuestionsPage.ClickNext();
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

        [Given(@"I click See results button")]
        [When(@"I click See results button")]
        public void WhenIClickSeeResultsButton()
        {
            _dysacPage.ClickSeeResults();
        }

        [When(@"I click See results button for ""(.*)""")]
        public void WhenIClickSeeResultsButtonFor(string jobCategory)
        {
            _answerMoreJobCategory = jobCategory;
            _yourResultsPage.ClickSeeResultsForJobCatergory(jobCategory);
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

        [When(@"I save progress")]
        [Given(@"I save progress")]
        public void GivenISaveProgress()
        {
            if (_scenarioContext.ScenarioInfo.Title.Contains("yes no questions"))
            {
                _yesNoQuestionsPage.ClickSaveMyProgress();
            }
            else
            {
                _dysacPage.ClickSaveProgress();
            }
        }

        [When(@"I choose the ""(.*)"" option of returning to assessment")]
        [Given(@"I choose the ""(.*)"" option of returning to assessment")]
        public void GivenIChooseTheOptionOfReturningToAssessment(string assessmentReturnOption)
        {
            if (_scenarioContext.ScenarioInfo.Title.Contains("yes no questions"))
            {
                _returnToAssessmentPage.SelectReferenceCode();
                _returnToAssessmentPage.ClickContinue();
            }
            else
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
        }

        [When(@"I make a note of the reference code")]
        [Given(@"I make a note of the reference code")]
        public void GivenIMakeANoteOfTheReferenceCode()
        {
            _yourReferenceCodePage.GetReferenceCode();
        }

        [Given(@"I use the reference code to return to my assessment from the Dysac home page")]
        [When(@"I use the reference code to return to my assessment from the Dysac home page")]
        public void WhenIUseTheReferenceCodeToReturnToMyAssessmentFromTheDysacHomePage()
        {
            _dysacPage.EnterYourReference(_yourReferenceCodePage.ReferenceCode);
            _dysacPage.ClickContinueToSaveProgress();
        }

        [Then(@"I am at the question where I left off")]
        [Then(@"I am at the question noted earlier before I saved progress")]
        public void ThenIAmAtTheQuestionNotedEarlierBeforeISavedProgress()
        {
            if (_scenarioContext.ScenarioInfo.Title.Contains("yes no questions"))
            {
                NUnit.Framework.Assert.AreEqual(_yesNoQuestionsPage.GetYesNoQuestion(), _currentQuestion, "Question displayed is incorrect");
            }
            else
            {
                NUnit.Framework.Assert.AreEqual(_percentCompleted + "%", _dysacPage.GetPercentageComplete(), "Reference code did not go to the correct percentage completed.");
            }
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

        [When(@"I click See matches in order to view the other career areas that might interest you")]
        public void WhenIClickSeeMatchesInOrderToViewTheOtherCareerAreasThatMightInterestYou()
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
            _answers = table.CreateSet<AnswersShowThat>().ToList();
            _yourResultsPage.AnswerQuestions(_answers);
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
            _expectedTraits = table.CreateSet<Traits>().ToList();

            switch (_scenarioContext.ScenarioInfo.Title)
            {
                case "TC23 - Real user interaction 1":
                    _yourResultsPage.VerifyTraits(_expectedTraits);
                    break;
                case "TC24 - Real user interaction 2":
                    _yourResultsPage.VerifyTraits(_expectedTraits);
                    break;
            }

            NUnit.Framework.Assert.True(_yourResultsPage.VerifyTraits(_expectedTraits), "What you told us trait(s) incorrect");
        }

        [Then(@"the traits appear in the same order as in the data table above")]
        public void ThenTheTraitsAppearInTheSameOrderAsInTheDataTableAbove()
        {
            NUnit.Framework.Assert.True(_yourResultsPage.AreTheyInSequence(_expectedTraits), "Trait(s) are not in the expected sequence");
        }

        [Then(@"the job roles appear in the same order as in the data table above")]
        public void ThenTheJobRolesAppearInTheSameOrderAsInTheDataTableAbove()
        {
            NUnit.Framework.Assert.True(_yourResultsPage.AreJobRolesInSequence(_expectedJobRoles, _answerMoreJobCategory), "Job roles(s) are not in the expected sequence for " + _answerMoreJobCategory + " category.");
        }

        [Then(@"the following job categories with their corresponding number of answer more questions are displayed")]
        public void ThenTheFollowingJobCategoriesWithTheirCorrespondingNumberOfAnswerMoreQuestionsAreDisplayed(Table table)
        {
            _yourResultsPage.ClickSeeMatches();
            IEnumerable<JobCategories> jobCategories = table.CreateSet<JobCategories>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyJobsAndNumberOfAnswers(jobCategories), "Job categories and or number for answer * more questions incorrect.");
        }

        [Given(@"I click the Answer ""(.*)"" more questions button for ""(.*)""")]
        [When(@"I click the Answer ""(.*)"" more questions button for ""(.*)""")]
        public void WhenIClickTheAnswerMoreQuestionsButtonFor(string numberOfQuestions, string jobCategory)
        {
            _answerMoreJobCategory = jobCategory;
            _yourResultsPage.ClickAnswerMoreQuestionsButton(numberOfQuestions, jobCategory);
        }

        [When(@"I go back and click the Answer ""(.*)"" more questions button for ""(.*)""")]
        public void WhenIGoBackAndClickTheAnswerMoreQuestionsButtonFor(string numberOfQuestions, string jobCategory)
        {
            _answerMoreJobCategory = jobCategory;
            _yourResultsPage.GoBackAnswering();
            _dysacPage.ClickStartAssessment();
            _yourResultsPage.AnswerQuestions(_answers);
            _assessmentCompletePage.ClickSeeResults();
            _yourResultsPage.ClickAnswerMoreQuestionsButton(numberOfQuestions, jobCategory);
        }

        [Given(@"the following question is displayed; ""(.*)""")]
        [Then(@"the following question is displayed; ""(.*)""")]
        public void ThenTheFollowingQuestionIsDisplayed(string question)
        {
            _currentQuestion = question;
            NUnit.Framework.Assert.AreEqual(question, _yesNoQuestionsPage.GetYesNoQuestion(), "Yes / No question is wrong.");
        }

        [Given(@"I make a note of this question")]
        public void GivenIMakeANoteOfThisQuestion()
        {
            /* current question noted in step '[Then(@"the following question is displayed; ""(.*)""")]' */
        }

        [Given(@"there are ""(.*)"" roles I might be interested in")]
        [Then(@"there are ""(.*)"" roles I might be interested in")]
        public void ThenThereAreRolesIMightBeInterestedIn(string numberOfRoles)
        {
            NUnit.Framework.Assert.AreEqual(numberOfRoles, _yourResultsPage.GetNumberOfRolesInterestedIn(_answerMoreJobCategory), "Number of roles stated as interested in for " + _answerMoreJobCategory + " are incorrect.");
        }

        [Then(@"I view the ""(.*)"" job category")]
        public void ThenIViewTheJobCategory(string jobCategory)
        {
            _answerMoreJobCategory = jobCategory;
            _yourResultsPage.ClickSeeResultsForJobCatergory(jobCategory);
        }

        [Given(@"I see the job roles")]
        [Then(@"I see the job roles")]
        public void ThenISeeTheJobRoles(Table table)
        {
            _expectedJobRoles = table.CreateSet<JobCategoryRoles>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyRoles(_expectedJobRoles, _answerMoreJobCategory), "Roles are incorrect for " + _answerMoreJobCategory + " job category.");
        }

        [Then(@"I see the job roles for the ""(.*)"" job category")]
        public void ThenISeeTheJobRolesForTheJobCategory(string jobCategory, Table table)
        {
            _expectedJobRoles = table.CreateSet<JobCategoryRoles>().ToList();
            NUnit.Framework.Assert.True(_yourResultsPage.VerifyRoles(_expectedJobRoles, jobCategory), "Roles are incorrect for " + jobCategory + " job category.");
        }

        [Then(@"the following message is displayed; ""(.*)""")]
        public void ThenTheFollowingMessageIsDisplayed(string noCareersMessage)
        {
            NUnit.Framework.Assert.AreEqual(noCareersMessage, _yourResultsPage.GetNoCareersMessage(_answerMoreJobCategory), "Roles you might be interested in are displayed; unexpectedly.");
        }

        [Given(@"I answer all the questions")]
        public void GivenIAnswerAllTheQuestions()
        {
            _yourResultsPage.AnswerQuestionsFromJson(filePath + "DataSet.json");
        }

        [Given(@"I answer all the questions using the data file ""(.*)""")]
        public void GivenIAnswerAllTheQuestionsUsingTheDataFile(string dataFile)
        {
            _yourResultsPage.AnswerQuestionsFromJson(filePath + dataFile + ".json");
            _assessmentCompletePage.ClickSeeResults();
        }

        [Given(@"I answer the next ""(.*)"" questions")]
        public void GivenIAnswerTheNextQuestions(int numberOfQuestionsToAnswer)
        {
            
        }

        [Given(@"I decide to change my answers")]
        public void GivenIDecideToChangeMyAnswers()
        {
            _yourResultsPage.ClickChangeMyAnswers(_answerMoreJobCategory);
        }

        [When(@"I check the description text beneath each suggested job category")]
        public void WhenICheckTheDescriptionTextBeneathEachSuggestedJobCategory()
        {
            _yourResultsPage.ClickSeeMatches();
            _yourResultsPage.GetJobCategories();
            _yourResultsPage.GetJobCategoryDescriptions();
        }

        [Then(@"each job category is mentioned as part of the narration of its corresponding descriptive text")]
        public void ThenEachJobCategoryIsMentionedAsPartOfTheNarrationOfItsCorrespondingDescriptiveText()
        {
            NUnit.Framework.Assert.IsTrue(_yourResultsPage.VerifyJobCategoryDescription(), "Description text does not match job category.");
        }

        [Given(@"I proceed to obtain a reference code")]
        public void GivenIProceedToObtainAReferenceCode()
        {
            _yourResultsPage.ClickContinue();
        }

        [Given(@"I make a note of the page that I am currently on")]
        public void GivenIMakeANoteOfThePageThatIAmCurrentlyOn()
        {
            _currentPage = _yourResultsPage.GetCurrentUrl();
            _yourResultsPage.ClickContinue();
        }

        [Given(@"I make a note of the suggested job categories")]
        public void GivenIMakeANoteOfTheSuggestedJobCategories()
        {
            _yourResultsPage.ClickSeeMatches();
            _jobCategoryCategoriesInitial  = _yourResultsPage.GetIWebElementText();
        }

        [Then(@"I am at the page noted earlier")]
        public void ThenIAmAtThePageNotedEarlier()
        {
            var qqq = _scenarioContext.GetWebDriver().Url;
            NUnit.Framework.Assert.AreEqual(_currentPage, _scenarioContext.GetWebDriver().Url, "You are not on the page noted earlier.");
        }

        [Then(@"the job categories are the same as those noted earlier")]
        public void ThenTheJobCategoriesAreTheSameAsThoseNotedEarlier()
        {
            NUnit.Framework.Assert.IsTrue(_yourResultsPage.CompareBeforeAfterJobCategories(_jobCategoryCategoriesInitial), "Job categories are not the same as those noted earlier.");
        }
    }
}