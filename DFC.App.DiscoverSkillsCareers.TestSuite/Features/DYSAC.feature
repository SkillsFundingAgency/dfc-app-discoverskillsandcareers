@webtest
Feature: DYSACUserActions

Background: 
	Given I load the DYSAC page

@DYSAC
Scenario: Starting Assessment loads the first questions
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do

@DYSAC
Scenario: TC01 - Starting Assessment loads the questions and show the percentage completion. Also displays the results
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I make decisions quickly
	And Percentage completion is 2%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to take control of situations
	And Percentage completion is 5%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I prefer to follow what other people are doing	
	And Percentage completion is 7%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like taking responsibility for other people	
	And Percentage completion is 10%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I set myself targets when I have things to do, and usually meet them	
	And Percentage completion is 12%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to see things through to the end	
	And Percentage completion is 15%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I think I am a competitive person	
	And Percentage completion is 17%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; Doing well in a career motivates me	
	And Percentage completion is 20%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I set myself goals in life	
	And Percentage completion is 22%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am comfortable talking people around to my way of thinking	
	And Percentage completion is 25%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am good at coming to an agreement with other people	
	And Percentage completion is 27%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am comfortable talking in front of a group of people	
	And Percentage completion is 30%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like meeting new people	
	And Percentage completion is 32%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I find it hard to understand other people's point of view	
	And Percentage completion is 35%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to help other people	
	And Percentage completion is 37%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy working with other people around me	
	And Percentage completion is 40%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I want to make things better for people	
	And Percentage completion is 42%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I will get involved if I think I can help	
	And Percentage completion is 45%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am comfortable hearing other people's problems	
	And Percentage completion is 47%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to work out complicated things	
	And Percentage completion is 50%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to get to the centre of the issue	
	And Percentage completion is 52%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like working with facts	
	And Percentage completion is 55%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like working with numbers	
	And Percentage completion is 57%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy learning new things	
	And Percentage completion is 60%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy coming up with new ways of doing things	
	And Percentage completion is 62%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I try to think differently to others	
	And Percentage completion is 65%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to use my imagination to create new things	
	And Percentage completion is 67%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to try new things	
	And Percentage completion is 70%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy creative activities	
	And Percentage completion is 72%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to focus on details	
	And Percentage completion is 75%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I plan my day so I can use my time best	
	And Percentage completion is 77%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like doing things in a careful order	
	And Percentage completion is 80%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to follow rules and processes	
	And Percentage completion is 82%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I feel restricted when I have to follow a routine	
	And Percentage completion is 85%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to see the results of the work I do	
	And Percentage completion is 87%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to get involved in making things	
	And Percentage completion is 90%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy getting involved in practical tasks	
	And Percentage completion is 92%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like working with my hands or tools	
	And Percentage completion is 95%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy planning a task more than actually doing it	
	And Percentage completion is 97%
	When I select Strongly agree option
	And I click Next
	Then The assessment is complete	
	And The task is 100% complete
	When I click See results button
	Then The results are displayed

@DYSAC
Scenario: TC02 - Display error message when moving to next question without selecting an option 
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I make decisions quickly
	And Percentage completion is 2%
	When I click Next
	Then The error is displayed; Choose an answer to the statement
	And Percentage completion is 2%

@DYSAC
Scenario: TC03 - Clicking back link takes to the previous question and updates the percentage completion 
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do
	And Percentage completion is 0%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I make decisions quickly
	And Percentage completion is 2%
	When I click Back to previous statement link
	Then The first question is displayed; I am comfortable telling people what they need to do
	And Percentage completion is 0%

@DYSAC
Scenario: TC04 - Saving progress and selecting reference code to return to the assessment displays reference code 
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do
	And Percentage completion is 0%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I make decisions quickly
	And Percentage completion is 2%
	When I save my progress
	And I select reference code to return to the assessment
	And I click continue
	Then The reference code is displayed

Scenario: TC05 - Progress bar displays correctly on each question
	And I click on Assessment
	When I select "Strongly agree" option and answer questions to the end
	Then the questions in turn have the following class attributes
	| Class attribute  |
	| ncs-progress__0  |
	| ncs-progress__2  |
	| ncs-progress__5  |
	| ncs-progress__7  |
	| ncs-progress__10 |
	| ncs-progress__12 |
	| ncs-progress__15 |
	| ncs-progress__17 |
	| ncs-progress__20 |
	| ncs-progress__22 |
	| ncs-progress__25 |
	| ncs-progress__27 |
	| ncs-progress__30 |
	| ncs-progress__32 |
	| ncs-progress__35 |
	| ncs-progress__37 |
	| ncs-progress__40 |
	| ncs-progress__42 |
	| ncs-progress__45 |
	| ncs-progress__47 |
	| ncs-progress__50 |
	| ncs-progress__52 |
	| ncs-progress__55 |
	| ncs-progress__57 |
	| ncs-progress__60 |
	| ncs-progress__62 |
	| ncs-progress__65 |
	| ncs-progress__67 |
	| ncs-progress__70 |
	| ncs-progress__72 |
	| ncs-progress__75 |
	| ncs-progress__77 |
	| ncs-progress__80 |
	| ncs-progress__82 |
	| ncs-progress__85 |
	| ncs-progress__87 |
	| ncs-progress__90 |
	| ncs-progress__92 |
	| ncs-progress__95 |
	| ncs-progress__97 |

Scenario Outline: TC06 - Return to assessment using reference code
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to <Percentage completed> percent
	When I save progress
	Then the "Send me an email with a link" radio button is present on the resultant page
	And the "Get a reference code" radio button is present on the resultant page
	When I choose the "Get a reference code" option of returning to assessment
	And I make a note of the reference code
	And I use the reference code to return to my assessment from the Dysac home page
	Then I am at the question where I left off
Examples: 
	| Percentage completed |
	| 70                   |
	| 2                    |
	| 30                   |

Scenario: TC07 - Current date is displayed on reference code page
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "10" percent
	And I save progress
	And I choose the "Get a reference code" option of returning to assessment
	When I view the date on the resultant page
	Then the date is todays date
	When I click the Return to assessment link
	Then I am at the question where I left off

Scenario Outline: TC08 - Phone number supplied appears on Check your phone page
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "7" percent
	And I save progress
	And I choose the "Get a reference code" option of returning to assessment
	When I supply phone number "07424037362"
	Then the phone number appears on the Check your phone page
	When I click the Return to assessment button
	Then I am at the question where I left off
	When I go Back and I click the Back to start link
	Then I am navigate to the Dysac home page

Scenario Outline: TC09 - Phone number field validation
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "2" percent
	And I save progress
	And I choose the "Get a reference code" option of returning to assessment
	When I supply phone number ""
	Then validation messages are displayed for the "phone number" field
	When I click Back 
	Then I am navigated to the "How would you like to return to your assessment?" page

Scenario: TC10 - Home page reference code field validation
	And I click continue without providing a reference
	Then validation messages are displayed for the "reference code" field

Scenario: TC11 - Email field validation and population
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "2" percent
	And I save progress
	And I choose the "Send me an email with a link" option of returning to assessment
	When I click the Back link from the "Email address" page
	Then I am navigated to the "How would you like to return to your assessment?" page
	And I go forward
	When I click Send on the resultant page without providing an email address
	Then validation messages are displayed for the "email address" field
	When I provide email address "victor.abegunde@methods.co.uk"
	Then I am navigated to the "Check your email" page
	And the email address used is present in the text on the page
	When I click the Back link from the "Check your email" page
	Then I am navigated to the "Email address" page
	And I go forward
	When I click the Return to assessment button
	Then I am at the question where I left off
	When I go Back and I click the Back to start link
	Then I am navigate to the Dysac home page 

Scenario: TC12 - All question radio button options are usable
	When I click on Assessment
	Then I am able to select the "Strongly agree" option for the "first" question
	And I am able to select the "Agree" option for the "second" question
	And I am able to select the "It depends" option for the "third" question
	And I am able to select the "Disagree" option for the "fourth" question
	And I am able to select the "Strongly disagree" option for the "fifth" question	

Scenario Outline: TC13 - Initial and all suggested job categories
	And I click on Assessment
	And I answer all questions selecting the <Answer option> option
	When I click See results button
	Then the job categories suggestions are <Initial job categories> in number
	And the initial job categories dispalyed are
	| Job category       |
	| Sports and leisure |
	| Manufacturing      |
	| Creative and media |
	When I click See matches to See 7 other career areas that might interest you
	Then the job categories suggestions are <All job categories> in number
	And all the job categories dispalyed are
	| Job category                   |
	| Sports and leisure             |
	| Manufacturing                  |
	| Creative and media             |
	| Construction and trades        |
	| Science and research           |
	| Business and finance           |
	| Emergency and uniform services |
	| Law and legal                  |
	| Teaching and education         |
	| Travel and tourism             |
	Examples: 
	| Answer option  | Initial job categories | All job categories |
	| Strongly agree | 3                      | 10                 |

Scenario Outline: TC14 - Number of answer more questions for each category are correct
	And I click on Assessment
	And I answer all questions selecting the <Answer option> option
	And I click See results button
	When I click See matches to See 7 other career areas that might interest you
	Then the following are the job categories suggested and their number of answer more questions
	| Job category                   | Number of answer more questions |
	| Sports and leisure             | 3                               |
	| Manufacturing                  | 4                               |
	| Creative and media             | 5                               |
	| Construction and trades        | 4                               |
	| Science and research           | 4                               |
	| Business and finance           | 3                               |
	| Emergency and uniform services | 3                               |
	| Law and legal                  | 3                               |
	| Teaching and education         | 3                               |
	| Travel and tourism             | 2                               |
	Examples: 
	| Answer option  |
	| Strongly agree |
