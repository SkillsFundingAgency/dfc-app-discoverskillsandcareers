@webtest
Feature: DYSACUserActions

Background: 
	Given I load the DYSAC page

@DYSAC
Scenario: Starting Assessment loads the first questions
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do

@smoke
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

@smoke
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

@smoke
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

@smoke
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

@DYSAC
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

@smoke
@DYSAC
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

@DYSAC
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

@smoke
@DYSAC
Scenario Outline: TC08 - Phone number supplied appears on Check your phone page
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "62" percent
	And I save progress
	And I choose the "Get a reference code" option of returning to assessment
	When I supply phone number "07424037362"
	Then the phone number appears on the Check your phone page
	When I click the Return to assessment button
	Then I am at the question where I left off
	When I go Back and I click the Back to start link
	Then I am navigate to the Dysac home page

@smoke
@DYSAC
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

@smoke
@DYSAC
Scenario: TC10 - Home page reference code field validation
	And I click continue without providing a reference
	Then validation messages are displayed for the "reference code" field

@smoke
@DYSAC
Scenario: TC11 - Email field validation and population
	And I click on Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "72" percent
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

@DYSAC
Scenario: TC12 - All question radio button options are usable
	When I click on Assessment
	Then I am able to select the "Strongly agree" option for the "first" question
	And I am able to select the "Agree" option for the "second" question
	And I am able to select the "It depends" option for the "third" question
	And I am able to select the "Disagree" option for the "fourth" question
	And I am able to select the "Strongly disagree" option for the "fifth" question	

@DYSAC
@ignore
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
	When I click See matches in order to view the other career areas that might interest you
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

@DYSAC
Scenario Outline: TC14 - Number of answer more questions for each category are correct
	And I click on Assessment
	And I answer all questions selecting the <Answer option> option
	And I click See results button
	When I click See matches in order to view the other career areas that might interest you
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

@smoke
@DYSAC
Scenario: TC15 - Driver-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you are motivated, set yourself personal goals and are comfortable competing with other people"

@DYSAC
Scenario: TC16 - Leader-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you like to lead other people and are good at taking control of situations"

@smoke
@DYSAC
Scenario: TC17 - Influencer-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Strongly agree    |
	| 30               | I am comfortable talking in front of a group of people               | Strongly agree    |
	| 32               | I like meeting new people                                            | Strongly agree    |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you are sociable and find it easy to understand people"

@DYSAC
Scenario: TC18 - Helper-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Strongly agree    |
	| 42               | I want to make things better for people                              | Strongly agree    |
	| 45               | I will get involved if I think I can help                            | Strongly agree    |
	| 47               | I am comfortable hearing other people's problems                     | Strongly agree    |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you enjoy helping and listening to other people"

@smoke
@DYSAC
Scenario: TC19 - Analyst-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly agree    |
	| 52               | I like to get to the centre of the issue                             | Strongly agree    |
	| 55               | I like working with facts                                            | Strongly agree    |
	| 57               | I like working with numbers                                          | Strongly agree    |
	| 60               | I enjoy learning new things                                          | Strongly agree    |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you like dealing with complicated problems or working with numbers"

@DYSAC
Scenario: TC20 - Creator-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Strongly agree    |
	| 67               | I like to use my imagination to create new things                    | Strongly agree    |
	| 70               | I like to try new things                                             | Strongly agree    |
	| 72               | I enjoy creative activities                                          | Strongly agree    |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you are a creative person and enjoy coming up with new ways of doing things"

@smoke
@DYSAC
Scenario: TC21 - Organiser-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the Your results page What you told us section displays the trait text "you like to plan things and are well organised"

@DYSAC
Scenario: TC22 - Doer-What you told us summary invoking answers
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the Your results page What you told us section displays the trait text "you are a practical person and enjoy getting things done"

@smoke
@DYSAC
Scenario: TC23 - Real user interaction 1
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | It depends        |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | It depends        |
	| 7                | I prefer to follow what other people are doing                       | Agree             |
	| 10               | I like taking responsibility for other people                        | Disagree          |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Agree             |
	| 17               | I think I am a competitive person                                    | Disagree          |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Agree             |
	| 25               | I am comfortable talking people around to my way of thinking         | Disagree          |
	| 27               | I am good at coming to an agreement with other people                | Disagree          |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | It depends        |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Disagree          |
	| 42               | I want to make things better for people                              | Strongly agree    |
	| 45               | I will get involved if I think I can help                            | Strongly agree    |
	| 47               | I am comfortable hearing other people's problems                     | Agree             |
	| 50               | I like to work out complicated things                                | Agree             |
	| 52               | I like to get to the centre of the issue                             | Agree             |
	| 55               | I like working with facts                                            | Strongly agree    |
	| 57               | I like working with numbers                                          | It depends        |
	| 60               | I enjoy learning new things                                          | Strongly agree    |
	| 62               | I enjoy coming up with new ways of doing things                      | Agree             |
	| 65               | I try to think differently to others                                 | Agree             |
	| 67               | I like to use my imagination to create new things                    | Disagree          |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Disagree          |
	| 75               | I like to focus on details                                           | Agree             |
	| 77               | I plan my day so I can use my time best                              | It depends        |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Agree             |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | It depends        |
	| 92               | I enjoy getting involved in practical tasks                          | It depends        |
	| 95               | I like working with my hands or tools                                | Agree             |
	| 97               | I enjoy planning a task more than actually doing it                  | It depends        |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                         |
	| you enjoy helping and listening to other people                    |
	| you like dealing with complicated problems or working with numbers |
	| you like to plan things and are well organised                     |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category         | Number of answer more questions |
	| Manufacturing        | 4                               |
	| Science and research | 4                               |
	| Travel and tourism   | 2                               |
	| Business and finance | 3                               |
	| Law and legal        | 3                               |
	| Animal care          | 2                               |
	| Delivery and storage | 2                               |
	| Healthcare           | 4                               |
	| Home services        | 3                               |
	| Transport            | 3                               |

@DYSAC
Scenario: TC24 - Real user interaction 2
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | It depends        |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | It depends        |
	| 7                | I prefer to follow what other people are doing                       | Agree             |
	| 10               | I like taking responsibility for other people                        | Disagree          |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Agree             |
	| 17               | I think I am a competitive person                                    | Disagree          |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Agree             |
	| 25               | I am comfortable talking people around to my way of thinking         | Disagree          |
	| 27               | I am good at coming to an agreement with other people                | Disagree          |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | It depends        |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Disagree          |
	| 42               | I want to make things better for people                              | Strongly agree    |
	| 45               | I will get involved if I think I can help                            | Strongly agree    |
	| 47               | I am comfortable hearing other people's problems                     | Agree             |
	| 50               | I like to work out complicated things                                | Agree             |
	| 52               | I like to get to the centre of the issue                             | Agree             |
	| 55               | I like working with facts                                            | Strongly agree    |
	| 57               | I like working with numbers                                          | It depends        |
	| 60               | I enjoy learning new things                                          | Strongly agree    |
	| 62               | I enjoy coming up with new ways of doing things                      | Agree             |
	| 65               | I try to think differently to others                                 | Agree             |
	| 67               | I like to use my imagination to create new things                    | Disagree          |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Disagree          |
	| 75               | I like to focus on details                                           | Agree             |
	| 77               | I plan my day so I can use my time best                              | It depends        |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Agree             |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | It depends        |
	| 92               | I enjoy getting involved in practical tasks                          | It depends        |
	| 95               | I like working with my hands or tools                                | Agree             |
	| 97               | I enjoy planning a task more than actually doing it                  | It depends        |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                         |
	| you enjoy helping and listening to other people                    |
	| you like dealing with complicated problems or working with numbers |
	| you like to plan things and are well organised                     |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category         | Number of answer more questions |
	| Manufacturing        | 4                               |
	| Science and research | 4                               |
	| Travel and tourism   | 2                               |
	| Business and finance | 3                               |
	| Law and legal        | 3                               |
	| Animal care          | 2                               |
	| Delivery and storage | 2                               |
	| Healthcare           | 4                               |
	| Home services        | 3                               |
	| Transport            | 3                               |

Scenario: TC25 - Leader-Driver
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you like to lead other people and are good at taking control of situations                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category | Number of answer more questions |
	| Managerial   | 4                               |
	When I click the Answer "4" more questions button for "Managerial"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you able to read well?                                                            | Yes    |
	| Are you comfortable working in a team with other people?                              | Yes    |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	| Are you good at thinking of new ways to do something without being told?              | Yes    |
	When I click See results button
	Then there are "19" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Human resources officer           |
	| Health and safety adviser         |
	| Estates officer                   |
	| Office manager                    |
	| Digital delivery manager          |
	| Management accountant             |
	| Travel agency manager             |
	| Customer services manager         |
	| Private practice accountant       |
	| Housing officer                   |
	| Supply chain manager              |
	| General practice surveyor         |
	| Rural surveyor                    |
	| Facilities manager                |
	| Civil Service executive officer   |
	| Planning and development surveyor |
	| Construction contracts manager    |
	| Public relations director         |
	| Security Service personnel        | 

Scenario: TC26 - Leader-Driver-Influencer
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Strongly agree    |
	| 30               | I am comfortable talking in front of a group of people               | Strongly agree    |
	| 32               | I like meeting new people                                            | Strongly agree    |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you like to lead other people and are good at taking control of situations                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	| you are sociable and find it easy to understand people                                         |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category | Number of answer more questions |
	| Managerial   | 4                               |
	When I click the Answer "4" more questions button for "Managerial"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	| Are you comfortable working in a team with other people?                              | Yes    |
	| Are you good at thinking of new ways to do something without being told?              | Yes    |
	| Are you able to read well?                                                            | Yes    |
	When I click See results button
	Then there are "19" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Human resources officer           |
	| Health and safety adviser         |
	| Estates officer                   |
	| Office manager                    |
	| Digital delivery manager          |
	| Management accountant             |
	| Travel agency manager             |
	| Customer services manager         |
	| Private practice accountant       |
	| Housing officer                   |
	| Supply chain manager              |
	| General practice surveyor         |
	| Rural surveyor                    |
	| Facilities manager                |
	| Civil Service executive officer   |
	| Planning and development surveyor |
	| Construction contracts manager    |
	| Public relations director         |
	| Security Service personnel        |

Scenario: TC27 - Leader-Driver-Influencer-Helper
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Strongly agree    |
	| 30               | I am comfortable talking in front of a group of people               | Strongly agree    |
	| 32               | I like meeting new people                                            | Strongly agree    |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Strongly agree    |
	| 42               | I want to make things better for people                              | Strongly agree    |
	| 45               | I will get involved if I think I can help                            | Strongly agree    |
	| 47               | I am comfortable hearing other people's problems                     | Strongly agree    |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you like to lead other people and are good at taking control of situations                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	| you are sociable and find it easy to understand people                                         |
	| you enjoy helping and listening to other people                                                |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category         | Number of answer more questions |
	| Managerial           | 4                               |
	| Hospitality and food | 2                               |
	| Retail and sales     | 4                               |
	| Social care          | 3                               |
	When I click the Answer "4" more questions button for "Managerial"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	| Are you comfortable working in a team with other people?                              | Yes    |
	| Are you good at thinking of new ways to do something without being told?              | Yes    |
	| Are you able to read well?                                                            | Yes    |
	When I click See results button
	Then there are "19" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Human resources officer           |
	| Health and safety adviser         |
	| Estates officer                   |
	| Office manager                    |
	| Digital delivery manager          |
	| Management accountant             |
	| Travel agency manager             |
	| Customer services manager         |
	| Private practice accountant       |
	| Housing officer                   |
	| Supply chain manager              |
	| General practice surveyor         |
	| Rural surveyor                    |
	| Facilities manager                |
	| Civil Service executive officer   |
	| Planning and development surveyor |
	| Construction contracts manager    |
	| Public relations director         |
	| Security Service personnel        |
	When I go back and click the Answer "2" more questions button for "Hospitality and food"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable working in a team with other people?                              | Yes    |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	When I click See results button
	Then there are "19" roles I might be interested in
	And I see the job roles
	| Job roles                    |
	| Catering manager             |
	| Counter service assistant    |
	| Publican                     |
	| Street food trader           |
	| Chef                         |
	| Cake decorator               |
	| Restaurant manager           |
	| Kitchen porter               |
	| Baker                        |
	| Butcher                      |
	| Bar person                   |
	| Hotel porter                 |
	| School lunchtime supervisor  |
	| Cruise ship steward          |
	| Fishmonger                   |
	| Butler                       |
	| Barista                      |
	| Wedding planner              |
	| Food manufacturing inspector |
	When I go back and click the Answer "4" more questions button for "Retail and sales"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	| Are you comfortable working in a team with other people?                              | Yes    |
	| Are you able to control your emotions even in difficult situations?                   | Yes    |
	| Do you think you are good at staying calm under pressure?                             | Yes    |
	When I click See results button
	Then there are "28" roles I might be interested in
	And I see the job roles
	| Job roles                            |
	| Call centre operator                 |
	| Customer service assistant           |
	| Shopkeeper                           |
	| Stock control assistant              |
	| Art valuer                           |
	| Pharmacist                           |
	| Construction plant hire adviser      |
	| Car rental agent                     |
	| Butcher                              |
	| Emergency medical dispatcher         |
	| Bar person                           |
	| Pharmacy assistant                   |
	| Retail manager                       |
	| Train station worker                 |
	| Bookmaker                            |
	| Leisure centre assistant             |
	| Pharmacy technician                  |
	| Customer services manager            |
	| Barista                              |
	| Shelf filler                         |
	| Events manager                       |
	| Airline customer service agent       |
	| Cabin crew                           |
	| Airport information assistant        |
	| Franchise owner                      |
	| Telephonist                          |
	| Tourist information centre assistant |
	| Builders' merchant                   |
	When I go back and click the Answer "3" more questions button for "Social care"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                                                     | Answer |
	| Are you able to control your emotions even in difficult situations?                                          | Yes    |
	| Would you be comfortable in a job where you would need to be sensitive to other people's needs and feelings? | Yes    |
	| Are you comfortable talking through things with other people so that they understand?                        | Yes    |
	When I click See results button
	Then there are "48" roles I might be interested in
	And I see the job roles
	| Job roles                                  |
	| Foster carer                               |
	| Social worker                              |
	| Residential support worker                 |
	| Substance misuse outreach worker           |
	| Funeral director                           |
	| Social work assistant                      |
	| Religious leader                           |
	| Money adviser                              |
	| Senior care worker                         |
	| Nanny                                      |
	| Nursery worker                             |
	| Playworker                                 |
	| Learning mentor                            |
	| Welfare rights officer                     |
	| Psychological wellbeing practitioner       |
	| Education welfare officer                  |
	| Communication support worker               |
	| Play therapist                             |
	| Family support worker                      |
	| Childminder                                |
	| Aid worker                                 |
	| Victim care officer                        |
	| Palliative care assistant                  |
	| Patient advice and liaison service officer |
	| Youth worker                               |
	| Nursery manager                            |
	| Care home manager                          |
	| Psychologist                               |
	| Psychotherapist                            |
	| Cognitive behavioural therapist            |
	| Care home advocate                         |
	| Horticultural therapist                    |
	| Forensic psychologist                      |
	| Family mediator                            |
	| Occupational therapist                     |
	| Counsellor                                 |
	| Child protection officer                   |
	| Careers adviser                            |
	| School crossing patrol                     |
	| School houseparent                         |
	| Life coach                                 |
	| Occupational therapy support worker        |
	| Clinical psychologist                      |
	| Music therapist                            |
	| Care worker                                |
	| Dramatherapist                             |
	| Care escort                                |
	| Art therapist                              |

Scenario: TC28 - Leader-Driver-Influencer-Helper-Organiser-Doer
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Strongly agree    |
	| 30               | I am comfortable talking in front of a group of people               | Strongly agree    |
	| 32               | I like meeting new people                                            | Strongly agree    |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Strongly agree    |
	| 42               | I want to make things better for people                              | Strongly agree    |
	| 45               | I will get involved if I think I can help                            | Strongly agree    |
	| 47               | I am comfortable hearing other people's problems                     | Strongly agree    |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you like to lead other people and are good at taking control of situations                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	| you are sociable and find it easy to understand people                                         |
	| you enjoy helping and listening to other people                                                |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                   | Number of answer more questions |
	| Business and finance           | 3                               |
	| Emergency and uniform services | 3                               |
	| Law and legal                  | 3                               |
	| Teaching and education         | 3                               |
	| Travel and tourism             | 2                               |
	| Animal care                    | 2                               |
	| Delivery and storage           | 2                               |
	| Healthcare                     | 4                               |
	| Home services                  | 3                               |
	| Managerial                     | 4                               |
	When I click the Answer "3" more questions button for "Business and finance"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | No     |
	| Are you confident solving maths problems using statistics, algebra and arithmetic?    | No     |
	| Are you able to read well?                                                            | No     |
	When I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles       |
	| Chief executive |
	When I go back and click the Answer "3" more questions button for "Emergency and uniform services"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you able to control your emotions even in difficult situations?                   | No     |
	| Do you think you are good at staying calm under pressure?                             | No     |
	| Are you comfortable talking through things with other people so that they understand? | No     |
	When I click See results button
	Then the following message is displayed; "No careers were found that might interest you based on your responses."
	When I go back and click the Answer "3" more questions button for "Law and legal"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	| Do you think you are good at staying calm under pressure?                             | No     |
	| Are you able to control your emotions even in difficult situations?                   | No     |
	When I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles           |
	| Court legal adviser |
	| Proofreader         |
	When I go back and click the Answer "3" more questions button for "Teaching and education"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | No     |
	| Are you able to control your emotions even in difficult situations?                   | No     |
	| Are you comfortable working in a team with other people?                              | Yes    |
	When I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles            |
	| Education technician |
	When I go back and click the Answer "2" more questions button for "Travel and tourism"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you able to control your emotions even in difficult situations?                   | Yes    |
	| Are you comfortable talking through things with other people so that they understand? | No     |
	When I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles      |
	| Airline pilot  |
	| Port operative |
	When I go back and click the Answer "2" more questions button for "Animal care"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you able to control your emotions even in difficult situations?                   | No     |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	When I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles          |
	| Countryside ranger |
	| Biologist          |
	When I go back and click the Answer "2" more questions button for "Delivery and storage"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                            | Answer |
	| Are you comfortable working in a team with other people?            | Yes    |
	| Are you able to control your emotions even in difficult situations? | Yes    |
	When I click See results button
	Then there are "16" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Order picker               |
	| Warehouse worker           |
	| Postperson                 |
	| Shelf filler               |
	| Import-export clerk        |
	| Road transport manager     |
	| Delivery van driver        |
	| Tanker driver              |
	| Packer                     |
	| Large goods vehicle driver |
	| Food packaging operative   |
	| Forklift driver            |
	| Airport baggage handler    |
	| Stock control assistant    |
	| Builders' merchant         |
	| Port operative             |
	When I go back and click the Answer "4" more questions button for "Healthcare"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                                                     | Answer |
	| Would you be comfortable in a job where you would need to be sensitive to other people's needs and feelings? | Yes    |
	| Are you able to control your emotions even in difficult situations?                                          | Yes    |
	| Are you comfortable working in a team with other people?                                                     | No     |
	| Are you comfortable talking through things with other people so that they understand?                        | No     |
	When I click See results button
	Then there are "5" roles I might be interested in
	And I see the job roles
	| Job roles         |
	| Radiographer      |
	| Dentist           |
	| Medical herbalist |
	| Naturopath        |
	| Homeopath         |
	When I go back and click the Answer "3" more questions button for "Home services"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                            | Answer |
	| Are you able to control your emotions even in difficult situations? | Yes    |
	| Are you comfortable working in a team with other people?            | No     |
	| Do you think you are good at staying calm under pressure?           | Yes    |
	When I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Window cleaner                    |
	| Industrial cleaner                |
	| Caretaker                         |
	| British Sign Language interpreter |
	When I go back and click the Answer "4" more questions button for "Managerial"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | No     |
	| Are you comfortable working in a team with other people?                              | No     |
	| Are you good at thinking of new ways to do something without being told?              | Yes    |
	| Are you able to read well?                                                            | Yes    |
	When I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles           |
	| Technical architect |

Scenario: TC29 - Leader-Influencer-Analyst-Organiser
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Strongly agree    |
	| 30               | I am comfortable talking in front of a group of people               | Strongly agree    |
	| 32               | I like meeting new people                                            | Strongly agree    |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly agree    |
	| 52               | I like to get to the centre of the issue                             | Strongly agree    |
	| 55               | I like working with facts                                            | Strongly agree    |
	| 57               | I like working with numbers                                          | Strongly agree    |
	| 60               | I enjoy learning new things                                          | Strongly agree    |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                 |
	| you like to lead other people and are good at taking control of situations |
	| you are sociable and find it easy to understand people                     |
	| you like dealing with complicated problems or working with numbers         |
	| you like to plan things and are well organised                             |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category        | Number of answer more questions |
	| Government services | 3                               |
	| Administration      | 3                               |
	When I click the Answer "3" more questions button for "Government services"
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Do you think you are good at staying calm under pressure?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Coastguard                 |
	| Probation services officer |
	When I go back and click the Answer "3" more questions button for "Administration"
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to read well?"
	When I select "No" answer and proceed
	And I click See results button
	Then the following message is displayed; "No careers were found that might interest you based on your responses."

Scenario: TC30 - Driver-Helper-Creator-Doer
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Strongly agree    |
	| 42               | I want to make things better for people                              | Strongly agree    |
	| 45               | I will get involved if I think I can help                            | Strongly agree    |
	| 47               | I am comfortable hearing other people's problems                     | Strongly agree    |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Strongly agree    |
	| 67               | I like to use my imagination to create new things                    | Strongly agree    |
	| 70               | I like to try new things                                             | Strongly agree    |
	| 72               | I enjoy creative activities                                          | Strongly agree    |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	| you enjoy helping and listening to other people                                                |
	| you are a creative person and enjoy coming up with new ways of doing things                    |
	| you are a practical person and enjoy getting things done                                       |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                | Number of answer more questions |
	| Animal care                 | 2                               |
	| Healthcare                  | 4                               |
	| Beauty and wellbeing        | 2                               |
	| Engineering and maintenance | 5                               |
	| Social care                 | 3                               |
	| Environment and land        | 4                               |
	When I click the Answer "2" more questions button for "Animal care"
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "20" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Zookeeper                  |
	| Animal care worker         |
	| Kennel worker              |
	| RSPCA inspector            |
	| Horse groom                |
	| Assistance dog trainer     |
	| Gamekeeper                 |
	| Racehorse trainer          |
	| Jockey                     |
	| Fish farmer                |
	| Dog groomer                |
	| Veterinary physiotherapist |
	| Veterinary nurse           |
	| Fishing boat deckhand      |
	| Vet                        |
	| Agricultural inspector     |
	| Horse riding instructor    |
	| Pet behaviour consultant   |
	| Pet shop assistant         |
	| Dog handler                |
	When I go back and click the Answer "4" more questions button for "Healthcare"
	Then the following question is displayed; "Would you be comfortable in a job where you would need to be sensitive to other people's needs and feelings?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "No" answer and proceed
	And I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                   |
	| Surgeon                     |
	| Cosmetic surgeon            |
	| Critical care technologist  |
	| Sterile services technician |
	When I go back and click the Answer "2" more questions button for "Beauty and wellbeing"
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Would you be comfortable in a job where you would need to be sensitive to other people's needs and feelings?"
	When I select "No" answer and proceed
	And I click See results button
	Then the following message is displayed; "No careers were found that might interest you based on your responses."
	When I go back and click the Answer "5" more questions button for "Engineering and maintenance"
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Do you think you are good at using words to describe ideas?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to do detailed, intricate work with your hands?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles   |
	| Model maker |
	| Electrician |
	When I go back and click the Answer "3" more questions button for "Social care"
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Would you be comfortable in a job where you would need to be sensitive to other people's needs and feelings?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "6" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Youth offending team officer      |
	| British Sign Language interpreter |
	| Housing officer                   |
	| Accommodation warden              |
	| Probation officer                 |
	| Equalities officer                |
	When I go back and click the Answer "4" more questions button for "Environment and land"
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to read well?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "5" roles I might be interested in
	And I see the job roles
	| Job roles                                                |
	| Meat hygiene inspector                                   |
	| Landscape architect                                      |
	| Agricultural inspector                                   |
	| Corporate responsibility and sustainability practitioner |
	| Food manufacturing inspector                             |

Scenario: TC31 - Analyst-Creator-Organiser-Doer - user journey
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly agree    |
	| 52               | I like to get to the centre of the issue                             | Strongly agree    |
	| 55               | I like working with facts                                            | Strongly agree    |
	| 57               | I like working with numbers                                          | Strongly agree    |
	| 60               | I enjoy learning new things                                          | Strongly agree    |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Strongly agree    |
	| 67               | I like to use my imagination to create new things                    | Strongly agree    |
	| 70               | I like to try new things                                             | Strongly agree    |
	| 72               | I enjoy creative activities                                          | Strongly agree    |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                  |
	| you like dealing with complicated problems or working with numbers          |
	| you are a creative person and enjoy coming up with new ways of doing things |
	| you like to plan things and are well organised                              |
	| you are a practical person and enjoy getting things done                    |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                      | Number of answer more questions |
	| Creative and media                | 5                               |
	| Construction and trades           | 4                               |
	| Delivery and storage              | 2                               |
	| Home services                     | 3                               |
	| Transport                         | 3                               |
	| Computing, technology and digital | 3                               |
	| Government services               | 3                               |
	| Engineering and maintenance       | 5                               |
	| Administration                    | 3                               |
	| Environment and land              | 4                               |
	When I click the Answer "5" more questions button for "Creative and media"
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you comfortable doing a variety of tasks in a job and open to things changing?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to read well?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "7" roles I might be interested in
	And I see the job roles
	| Job roles                    |
	| Newspaper journalist         |
	| Film critic                  |
	| Broadcast journalist         |
	| Magazine journalist          |
	| Media researcher             |
	| Vlogger                      |
	| Market research data analyst |
	When I view the "Administration" job category
	Then there are "3" roles I might be interested in
	And I see the job roles
	| Job roles            |
	| Trade union official |
	| Insurance broker     |
	| Interpreter          |
	When I click the Answer "2" more questions button for "Construction and trades"
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to do detailed, intricate work with your hands?"
	When I select "No" answer and proceed
	And I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles   |
	| Drone pilot |
	When I view the "Delivery and storage" job category
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles |
	| Roadie    |
	When I click the Answer "1" more questions button for "Home services"
	Then the following question is displayed; "Do you think you are good at staying calm under pressure?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Window cleaner                    |
	| Industrial cleaner                |
	| Caretaker                         |
	| British Sign Language interpreter |
	When I view the "Government services" job category
	Then there are "33" roles I might be interested in
	And I see the job roles
	| Job roles                                                     |
	| Civil enforcement officer                                     |
	| RAF airman or airwoman                                        |
	| Civil Service executive officer                               |
	| Immigration officer                                           |
	| Soldier                                                       |
	| Civil Service manager                                         |
	| Scenes of crime officer                                       |
	| Police officer                                                |
	| Royal Marines commando                                        |
	| Neighbourhood warden                                          |
	| RAF officer                                                   |
	| Police community support officer                              |
	| RAF non-commissioned aircrew                                  |
	| Prison governor                                               |
	| Royal Marines officer                                         |
	| Royal Navy officer                                            |
	| Royal Navy rating                                             |
	| Army officer                                                  |
	| Environmental health officer                                  |
	| Bodyguard                                                     |
	| Assistant immigration officer                                 |
	| Registrar of births, deaths, marriages and civil partnerships |
	| Food manufacturing inspector                                  |
	| Border Force officer                                          |
	| Dog handler                                                   |
	| Chief inspector                                               |
	| Fingerprint officer                                           |
	| Probation officer                                             |
	| Heritage officer                                              |
	| Security Service personnel                                    |
	| Child protection officer                                      |
	| School crossing patrol                                        |
	| Diver                                                         |
	When I click the Answer "2" more questions button for "Computing, technology and digital"
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to carry on with a task even if it starts to get difficult?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles        |
	| Technical author |
	| Vlogger          |
	When I view the "Environment and land" job category
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles   |
	| Gamekeeper  |
	| Drone pilot |
	When I click the Answer "1" more questions button for "Engineering and maintenance"
	Then the following question is displayed; "Do you think you are good at using words to describe ideas?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles                       |
	| Electricity generation worker   |
	| Electricity distribution worker |

Scenario: TC32 - Helper-Analyst-Creator - user journey
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly agree    |
	| 52               | I like to get to the centre of the issue                             | Strongly agree    |
	| 55               | I like working with facts                                            | Strongly agree    |
	| 57               | I like working with numbers                                          | Strongly agree    |
	| 60               | I enjoy learning new things                                          | Strongly agree    |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Strongly agree    |
	| 67               | I like to use my imagination to create new things                    | Strongly agree    |
	| 70               | I like to try new things                                             | Strongly agree    |
	| 72               | I enjoy creative activities                                          | Strongly agree    |
	| 75               | I like to focus on details                                           | Strongly disagree |
	| 77               | I plan my day so I can use my time best                              | Strongly disagree |
	| 80               | I like doing things in a careful order                               | Strongly disagree |
	| 82               | I like to follow rules and processes                                 | Strongly disagree |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly agree    |
	| 87               | I like to see the results of the work I do                           | Strongly disagree |
	| 90               | I like to get involved in making things                              | Strongly disagree |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly disagree |
	| 95               | I like working with my hands or tools                                | Strongly disagree |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly agree    |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                  |
	| you like dealing with complicated problems or working with numbers          |
	| you are a creative person and enjoy coming up with new ways of doing things |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                      | Number of answer more questions |
	| Computing, technology and digital | 3                               |
	When I click the Answer "3" more questions button for "Computing, technology and digital"
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to read well?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to carry on with a task even if it starts to get difficult?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "16" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Test lead                  |
	| Technical architect        |
	| Database administrator     |
	| Web content manager        |
	| IT security co-ordinator   |
	| Network manager            |
	| Forensic computer analyst  |
	| Cyber intelligence officer |
	| Web developer              |
	| Media researcher           |
	| Archivist                  |
	| Web designer               |
	| Data scientist             |
	| Operational researcher     |
	| Business analyst           |
	| Robotics engineer          |

Scenario: TC33 - Organiser-Doer - user journey
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly disagree |
	| 65               | I try to think differently to others                                 | Strongly disagree |
	| 67               | I like to use my imagination to create new things                    | Strongly disagree |
	| 70               | I like to try new things                                             | Strongly disagree |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                               |
	| you like to plan things and are well organised           |
	| you are a practical person and enjoy getting things done |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                | Number of answer more questions |
	| Delivery and storage        | 2                               |
	| Home services               | 3                               |
	| Transport                   | 3                               |
	| Government services         | 3                               |
	| Engineering and maintenance | 5                               |
	| Administration              | 3                               |
	| Environment and land        | 4                               |
	When I click the Answer "2" more questions button for "Delivery and storage"
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles |
	| Roadie    |
	When I click the Answer "1" more questions button for "Home services"
	Then the following question is displayed; "Do you think you are good at staying calm under pressure?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Window cleaner                    |
	| Industrial cleaner                |
	| Caretaker                         |
	| British Sign Language interpreter |
	When I click See results button for "Transport"
	Then there are "3" roles I might be interested in
	And I see the job roles
	| Job roles              |
	| Fishing vessel skipper |
	| Airline pilot          |
	| Tractor driver         |
	When I click the Answer "1" more questions button for "Government services"
	And I select "No" answer and proceed
	And I click See results button
	Then I see the job roles
	| Job roles                  |
	| Coastguard                 |
	| Probation services officer |
	When I click the Answer "3" more questions button for "Engineering and maintenance"
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Do you think you are good at using words to describe ideas?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you able to do detailed, intricate work with your hands?"
	When I select "No" answer and proceed
	And I click See results button
	Then I see the job roles
	| Job roles                       |
	| Electricity generation worker   |
	| Electricity distribution worker |
	When I click the Answer "1" more questions button for "Administration"
	Then the following question is displayed; "Are you able to read well?"
	When I select "No" answer and proceed
	And I click See results button
	Then I see the job roles
	| Job roles              |
	| Purchasing manager     |
	| Reprographic assistant |
	| Finance officer        |
	When I click See results button for "Environment and land"
	Then I see the job roles
	| Job roles                   |
	| Thermal insulation engineer |

Scenario: TC34 - Creator-Organiser-Doer - user journey
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly disagree |
	| 2                | I make decisions quickly                                             | Strongly disagree |
	| 5                | I like to take control of situations                                 | Strongly disagree |
	| 7                | I prefer to follow what other people are doing                       | Strongly agree    |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly disagree |
	| 15               | I like to see things through to the end                              | Strongly disagree |
	| 17               | I think I am a competitive person                                    | Strongly disagree |
	| 20               | Doing well in a career motivates me                                  | Strongly disagree |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly disagree |
	| 27               | I am good at coming to an agreement with other people                | Strongly disagree |
	| 30               | I am comfortable talking in front of a group of people               | Strongly disagree |
	| 32               | I like meeting new people                                            | Strongly disagree |
	| 35               | I find it hard to understand other people's point of view            | Strongly agree    |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Strongly agree    |
	| 67               | I like to use my imagination to create new things                    | Strongly agree    |
	| 70               | I like to try new things                                             | Strongly agree    |
	| 72               | I enjoy creative activities                                          | Strongly agree    |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                  |
	| you are a creative person and enjoy coming up with new ways of doing things |
	| you like to plan things and are well organised                              |
	| you are a practical person and enjoy getting things done                    |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                | Number of answer more questions |
	| Delivery and storage        | 2                               |
	| Home services               | 3                               |
	| Transport                   | 3                               |
	| Government services         | 3                               |
	| Engineering and maintenance | 5                               |
	| Administration              | 3                               |
	| Environment and land        | 4                               |
	When I click the Answer "2" more questions button for "Delivery and storage"
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles |
	| Roadie    |
	When I click the Answer "1" more questions button for "Home services"
	Then the following question is displayed; "Do you think you are good at staying calm under pressure?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Window cleaner                    |
	| Industrial cleaner                |
	| Caretaker                         |
	| British Sign Language interpreter |
	When I click See results button for "Transport"
	Then there are "3" roles I might be interested in
	And I see the job roles
	| Job roles              |
	| Fishing vessel skipper |
	| Airline pilot          |
	| Tractor driver         |
	When I click the Answer "1" more questions button for "Government services"
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "No" answer and proceed
	And I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Coastguard                 |
	| Probation services officer |
	When I click the Answer "3" more questions button for "Engineering and maintenance"
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Do you think you are good at using words to describe ideas?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to do detailed, intricate work with your hands?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then the following message is displayed; "No careers were found that might interest you based on your responses."
	When I click the Answer "1" more questions button for "Administration"
	Then the following question is displayed; "Are you able to read well?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles                 |
	| Trading standards officer |
	
Scenario: TC35 - Leader-Driver-Influencer-Creator-Organiser-Doer - user journey
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Strongly agree    |
	| 5                | I like to take control of situations                                 | Strongly agree    |
	| 7                | I prefer to follow what other people are doing                       | Strongly disagree |
	| 10               | I like taking responsibility for other people                        | Strongly agree    |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Strongly agree    |
	| 17               | I think I am a competitive person                                    | Strongly agree    |
	| 20               | Doing well in a career motivates me                                  | Strongly agree    |
	| 22               | I set myself goals in life                                           | Strongly agree    |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Strongly agree    |
	| 30               | I am comfortable talking in front of a group of people               | Strongly agree    |
	| 32               | I like meeting new people                                            | Strongly agree    |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly disagree |
	| 40               | I enjoy working with other people around me                          | Strongly disagree |
	| 42               | I want to make things better for people                              | Strongly disagree |
	| 45               | I will get involved if I think I can help                            | Strongly disagree |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly disagree |
	| 52               | I like to get to the centre of the issue                             | Strongly disagree |
	| 55               | I like working with facts                                            | Strongly disagree |
	| 57               | I like working with numbers                                          | Strongly disagree |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Strongly agree    |
	| 67               | I like to use my imagination to create new things                    | Strongly agree    |
	| 70               | I like to try new things                                             | Strongly agree    |
	| 72               | I enjoy creative activities                                          | Strongly agree    |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Strongly agree    |
	| 80               | I like doing things in a careful order                               | Strongly agree    |
	| 82               | I like to follow rules and processes                                 | Strongly agree    |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Strongly agree    |
	| 92               | I enjoy getting involved in practical tasks                          | Strongly agree    |
	| 95               | I like working with my hands or tools                                | Strongly agree    |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you like to lead other people and are good at taking control of situations                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	| you are sociable and find it easy to understand people                                         |
	| you are a creative person and enjoy coming up with new ways of doing things                    |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                | Number of answer more questions |
	| Business and finance        | 3                               |
	| Law and legal               | 3                               |
	| Delivery and storage        | 2                               |
	| Home services               | 3                               |
	| Managerial                  | 4                               |
	| Transport                   | 3                               |
	| Beauty and wellbeing        | 2                               |
	| Government services         | 3                               |
	| Engineering and maintenance | 5                               |
	| Administration              | 3                               |
	When I click the Answer "3" more questions button for "Business and finance"
	Then the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	When I select "Yes" answer and proceed to the next question
	Then the following question is displayed; "Are you confident solving maths problems using statistics, algebra and arithmetic?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to read well?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "6" roles I might be interested in
	And I see the job roles
	| Job roles                                                |
	| Insurance loss adjuster                                  |
	| Payroll manager                                          |
	| Local government revenues officer                        |
	| Insurance technician                                     |
	| Corporate responsibility and sustainability practitioner |
	| School business manager                                  |
	When I click the Answer "2" more questions button for "Law and legal"
	Then the following question is displayed; "Do you think you are good at staying calm under pressure?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Are you able to control your emotions even in difficult situations?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles         |
	| Company secretary |
	When I view the "Government services" job category
	Then there are "5" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Diplomatic Service officer |
	| Prison instructor          |
	| Housing policy officer     |
	| Air accident investigator  |
	| Careers adviser            |
	When I click the Answer "1" more questions button for "Delivery and storage"
	Then the following question is displayed; "Are you comfortable working in a team with other people?"
	When I select "No" answer and proceed
	And I click See results button
	Then there are "1" roles I might be interested in
	And I see the job roles
	| Job roles |
	| Roadie    |
	When I view the "Transport" job category
	Then I see the job roles
	| Job roles                 |
	| Driving instructor        |
	| Helicopter engineer       |
	| Air accident investigator |
	When I view the "Administration" job category
	Then there are "3" roles I might be interested in
	And I see the job roles
	| Job roles            |
	| Trade union official |
	| Insurance broker     |
	| Interpreter          |
	When I view the "Home services" job category
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles     |
	| Cleaner       |
	| Chimney sweep |
	When I click the Answer "1" more questions button for "Managerial"
	Then the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	When I select "No" answer and proceed
	And I click See results button
	Then there are "7" roles I might be interested in
	And I see the job roles
	| Job roles                |
	| MP                       |
	| Payroll manager          |
	| Nursery manager          |
	| Bank manager             |
	| Credit manager           |
	| Building control officer |
	| Tax inspector            |
	When I click the Answer "1" more questions button for "Beauty and wellbeing"
	Then the following question is displayed; "Would you be comfortable in a job where you would need to be sensitive to other people's needs and feelings?"
	When I select "No" answer and proceed
	And I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                  |
	| Hairdresser                |
	| Nail technician            |
	| Pilates teacher            |
	| Tattooist and body piercer |
	When I click the Answer "3" more questions button for "Engineering and maintenance"
	Then the following question is displayed; "Are you comfortable analysing information to solve problems?"
	When I select "No" answer and proceed to the next question
	Then the following question is displayed; "Do you think you are good at using words to describe ideas?"
	When I select "No" answer and proceed
	Then the following question is displayed; "Are you able to do detailed, intricate work with your hands?"
	When I select "Yes" answer and proceed
	And I click See results button
	Then there are "4" roles I might be interested in
	And I see the job roles
	| Job roles                   |
	| Security systems installer  |
	| Engineering craft machinist |
	| Vending machine operator    |
	| Diver                       |

Scenario: TC36 - I want to save my progress during yes no questions
	And I click on Assessment
	And I answer all the questions using the data file "AnswerSetTC36"
	And I click the Answer "4" more questions button for "Managerial"
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I make a note of this question
	And I save progress
	And I choose the "Get a reference code" option of returning to assessment
	And I make a note of the reference code
	When I use the reference code to return to my assessment from the Dysac home page
	Then I am at the question noted earlier before I saved progress

Scenario: TC37 - I want to change my answers after initial job category suggestion
	And I click on Assessment
	And I answer all the questions using the data file "AnswerSetTC36"
	And I click the Answer "4" more questions button for "Managerial"
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you able to read well?"
	And I select "Yes" answer and proceed
	And I click See results button
	And there are "1" roles I might be interested in
	And I see the job roles
	| Job roles           |
	| Technical architect |
	But I decide to change my answers
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you able to read well?"
	And I select "No" answer and proceed
	And I click See results button
	And there are "1" roles I might be interested in
	And I see the job roles
	| Job roles                   |
	| Textiles production manager |

Scenario: TC38 - I want to change my answers after initial job category suggestion
	And I click on Assessment
	And I answer all the questions using the data file "AnswerSetTC36"
	And I click the Answer "4" more questions button for "Managerial"
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you able to read well?"
	And I select "Yes" answer and proceed
	And I click See results button
	And there are "1" roles I might be interested in
	And I see the job roles
	| Job roles       |
	| Network manager |
	But I decide to change my answers
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you able to read well?"
	And I select "Yes" answer and proceed
	And I click See results button
	And there are "1" roles I might be interested in
	And I see the job roles
	| Job roles           |
	| Technical architect |

Scenario: TC39 - I want to change my answers after initial job category suggestion
	And I click on Assessment
	And I answer all the questions using the data file "AnswerSetTC36"
	And I click the Answer "4" more questions button for "Managerial"
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you able to read well?"
	And I select "Yes" answer and proceed
	And I click See results button
	And there are "12" roles I might be interested in
	And I see the job roles
	| Job roles                        |
	| Bid writer                       |
	| GP practice manager              |
	| Community education co-ordinator |
	| Town planner                     |
	| Headteacher                      |
	| Care home manager                |
	| Surveying technician             |
	| Health service manager           |
	| Diplomatic Service officer       |
	| Museum curator                   |
	| Wedding planner                  |
	| Events manager                   |
	But I decide to change my answers
	And the following question is displayed; "Are you comfortable talking through things with other people so that they understand?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you comfortable working in a team with other people?"
	And I select "No" answer and proceed to the next question
	And the following question is displayed; "Are you good at thinking of new ways to do something without being told?"
	And I select "Yes" answer and proceed to the next question
	And the following question is displayed; "Are you able to read well?"
	And I select "Yes" answer and proceed
	And I click See results button
	And there are "8" roles I might be interested in
	And I see the job roles
	| Job roles                    |
	| Estimator                    |
	| Advertising media planner    |
	| Environmental consultant     |
	| Consumer scientist           |
	| Economist                    |
	| Business development manager |
	| Quantity surveyor            |
	| Marketing manager            |

Scenario: TC40 - I want to check that descriptive texts match suggested job categories
	And I click on Assessment
	And I answer all the questions using the data file "AnswerSetTC38"
	When I check the description text beneath each suggested job category
	Then each job category is mentioned as part of the narration of its corresponding descriptive text

Scenario: TC41 - I want to be able to use a reference code to see my results
	And I click on Assessment
	And I answer all the questions using the data file "AnswerSetTC38"
	And I make a note of the suggested job categories
	And I make a note of the page that I am currently on
	And I choose the "Get a reference code" option of returning to assessment
	And I make a note of the reference code
	When I use the reference code to return to my assessment from the Dysac home page
	Then I am at the page noted earlier
	And the job categories are the same as those noted earlier

Scenario: TC42 - All options selected
	And I click on Assessment
	And I provide the following answers to the resultant questions
	| Percent progress | Question                                                             | Answer            |
	| 0                | I am comfortable telling people what they need to do                 | Strongly agree    |
	| 2                | I make decisions quickly                                             | Agree             |
	| 5                | I like to take control of situations                                 | It depends        |
	| 7                | I prefer to follow what other people are doing                       | Disagree          |
	| 10               | I like taking responsibility for other people                        | Strongly disagree |
	| 12               | I set myself targets when I have things to do, and usually meet them | Strongly agree    |
	| 15               | I like to see things through to the end                              | Agree             |
	| 17               | I think I am a competitive person                                    | It depends        |
	| 20               | Doing well in a career motivates me                                  | Disagree          |
	| 22               | I set myself goals in life                                           | Strongly disagree |
	| 25               | I am comfortable talking people around to my way of thinking         | Strongly agree    |
	| 27               | I am good at coming to an agreement with other people                | Agree             |
	| 30               | I am comfortable talking in front of a group of people               | It depends        |
	| 32               | I like meeting new people                                            | Disagree          |
	| 35               | I find it hard to understand other people's point of view            | Strongly disagree |
	| 37               | I like to help other people                                          | Strongly agree    |
	| 40               | I enjoy working with other people around me                          | Agree             |
	| 42               | I want to make things better for people                              | It depends        |
	| 45               | I will get involved if I think I can help                            | Disagree          |
	| 47               | I am comfortable hearing other people's problems                     | Strongly disagree |
	| 50               | I like to work out complicated things                                | Strongly agree    |
	| 52               | I like to get to the centre of the issue                             | Agree             |
	| 55               | I like working with facts                                            | It depends        |
	| 57               | I like working with numbers                                          | Disagree          |
	| 60               | I enjoy learning new things                                          | Strongly disagree |
	| 62               | I enjoy coming up with new ways of doing things                      | Strongly agree    |
	| 65               | I try to think differently to others                                 | Agree             |
	| 67               | I like to use my imagination to create new things                    | It depends        |
	| 70               | I like to try new things                                             | Disagree          |
	| 72               | I enjoy creative activities                                          | Strongly disagree |
	| 75               | I like to focus on details                                           | Strongly agree    |
	| 77               | I plan my day so I can use my time best                              | Agree             |
	| 80               | I like doing things in a careful order                               | It depends        |
	| 82               | I like to follow rules and processes                                 | Disagree          |
	| 85               | I feel restricted when I have to follow a routine                    | Strongly disagree |
	| 87               | I like to see the results of the work I do                           | Strongly agree    |
	| 90               | I like to get involved in making things                              | Agree             |
	| 92               | I enjoy getting involved in practical tasks                          | It depends        |
	| 95               | I like working with my hands or tools                                | Disagree          |
	| 97               | I enjoy planning a task more than actually doing it                  | Strongly disagree |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                               |
	| you are sociable and find it easy to understand people   |
	| you like to plan things and are well organised           |
	| you are a practical person and enjoy getting things done |
	And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                | Number of answer more questions |
	| Delivery and storage        | 2                               |
	| Home services               | 3                               |
	| Transport                   | 3                               |
	| Government services         | 3                               |
	| Engineering and maintenance | 5                               |
	| Administration              | 3                               |
	| Environment and land        | 4                               |

Scenario Outline: TC43 - Checks for Your result headings
	And I click on Assessment
	When I answer all the questions using the data file <Answer set>
	Then the job categories section is titled <no. of Careers areas> career areas(s) that might interest you 
	And the other careers areas section is titled See <no. of Other careers areas> other career area(s) that might interest you 
	Examples: 
	| no. | Answer set            | no. of Careers areas | no. of Other careers areas |
	| 1   | AnswerSetTC36         | 4                    | 1                          |
	| 2   | AnswerSetTC38         | 10                   | 7                          |
	| 3   | AnswerSetCompareIII   | 10                   | 7                          |
	| 4   | AnswerSetTC25         | 1                    | 0                          |
	| 5   | AnswerSetTC29Capture5 | 2                    | 0                          |
	| 6   | AnswerSetTC30Capture6 | 6                    | 3                          |

Scenario Outline: TC44 - Dataflow example
	And I click on Assessment
	And I answer all questions selecting the <Answer option> option
	And I click See results button
	When I click See matches in order to view the other career areas that might interest you
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you are motivated, set yourself personal goals and are comfortable competing with other people |
	| you enjoy helping and listening to other people                                                |
	| you like dealing with complicated problems or working with numbers                             |
	| you are a creative person and enjoy coming up with new ways of doing things                    |
	And the following are the job categories suggested and their number of answer more questions
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
	When I click the Answer "4" more questions button for "Manufacturing"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                            | Answer |
	| Do you think you are good at using words to describe ideas?         | Yes    |
	| Are you comfortable working in a team with other people?            | Yes    |
	| Are you able to control your emotions even in difficult situations? | Yes    |
	| Are you able to do detailed, intricate work with your hands?        | Yes    |
	When I click See results button
	Then there are "10" roles I might be interested in
	And I see the job roles
	| Job roles                         |
	| Production worker (manufacturing) |
	| Leather technologist              |
	| Pattern cutter                    |
	| Dressmaker                        |
	| Foundry mould maker               |
	| Foundry process operator          |
	| Food packaging operative          |
	| Garment technologist              |
	| Bottler                           |
	| Quarry worker                     |
	When I click the Answer "4" more questions button for "Creative and media"
	Then I provide the corresponding answers to the following questions as they are displayed in turn
	| Question                                                                              | Answer |
	| Are you comfortable talking through things with other people so that they understand? | Yes    |
	| Are you good at thinking of new ways to do something without being told?              | No     |
	| Are you comfortable doing a variety of tasks in a job and open to things changing?    | No     |
	| Are you able to read well?                                                            | Yes    |
	When I click See results button
	Then there are "2" roles I might be interested in
	And I see the job roles
	| Job roles        |
	| Art valuer       |
	| Technical author |

	Examples: 
	| Answer option  |
	| Strongly agree |