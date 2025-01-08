@webtest
Feature: DYSACUserActions

Background: 
	Given I load the DYSAC page

@DYSAC
Scenario: Starting Assessment loads the first questions
	When I click on start skills Assessment
	And I click on start your Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do

@smoke
@DYSAC
Scenario: TC01 - Starting Assessment loads the questions and show the percentage completion. Also displays the results
    When I click on start skills Assessment
	And I click on start your Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I make decisions quickly
	And Percentage completion is 2%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to take control of situations
	And Percentage completion is 4%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I prefer to follow what other people are doing	
	And Percentage completion is 7%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like taking responsibility for other people	
	And Percentage completion is 9%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I set myself targets when I have things to do, and usually meet them	
	And Percentage completion is 12%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to see things through to the end	
	And Percentage completion is 14%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I think I am a competitive person	
	And Percentage completion is 17%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; Doing well in a career motivates me	
	And Percentage completion is 19%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I set myself goals in life	
	And Percentage completion is 21%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am comfortable talking people around to my way of thinking	
	And Percentage completion is 24%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am good at coming to an agreement with other people	
	And Percentage completion is 26%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am comfortable talking in front of a group of people	
	And Percentage completion is 29%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like meeting new people	
	And Percentage completion is 31%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I find it hard to understand other people's point of view	
	And Percentage completion is 34%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to help other people	
	And Percentage completion is 36%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy working with other people around me	
	And Percentage completion is 39%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I want to make things better for people	
	And Percentage completion is 41%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I will get involved if I think I can help	
	And Percentage completion is 43%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I am comfortable hearing other people's problems	
	And Percentage completion is 46%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to work out complicated things	
	And Percentage completion is 48%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to get to the centre of the issue	
	And Percentage completion is 51%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like working with facts	
	And Percentage completion is 53%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like working with numbers	
	And Percentage completion is 56%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy learning new things	
	And Percentage completion is 58%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy coming up with new ways of doing things	
	And Percentage completion is 60%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I try to think differently to others	
	And Percentage completion is 63%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to use my imagination to create new things	
	And Percentage completion is 65%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to try new things	
	And Percentage completion is 68%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy creative activities	
	And Percentage completion is 70%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to focus on details	
	And Percentage completion is 73%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I plan my day so I can use my time best	
	And Percentage completion is 75%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like doing things in a careful order	
	And Percentage completion is 78%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to follow rules and processes	
	And Percentage completion is 80%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I feel restricted when I have to follow a routine	
	And Percentage completion is 82%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to see the results of the work I do	
	And Percentage completion is 85%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like to get involved in making things	
	And Percentage completion is 87%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy getting involved in practical tasks	
	And Percentage completion is 90%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I like working with my hands or tools	
	And Percentage completion is 92%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I enjoy planning a task more than actually doing it	
	And Percentage completion is 95%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; test question	
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
    When I click on start skills Assessment	
	And I click on start your Assessment
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
    When I click on start skills Assessment	
	And I click on start your Assessment
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
	When I click on start skills Assessment	
	And I click on start your Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do
	And Percentage completion is 0%
	When I select Strongly agree option
	And I click Next
	Then The next question is displayed; I make decisions quickly
	And Percentage completion is 2%
	When I save my progress
	Then The reference code is displayed

@DYSAC
Scenario: TC05 - Progress bar displays correctly on each question
	When I click on start skills Assessment	
	And I click on start your Assessment
	When I select "Strongly agree" option and answer questions to the end
	Then the questions in turn have the following class attributes
	| Class attribute  |
	| ncs-progress__0  |
	| ncs-progress__2  |
	| ncs-progress__4  |
	| ncs-progress__7  |
	| ncs-progress__9 |
	| ncs-progress__12 |
	| ncs-progress__14 |
	| ncs-progress__17 |
	| ncs-progress__19 |
	| ncs-progress__21 |
	| ncs-progress__24 |
	| ncs-progress__26 |
	| ncs-progress__29 |
	| ncs-progress__31 |
	| ncs-progress__34 |
	| ncs-progress__36 |
	| ncs-progress__39 |
	| ncs-progress__41 |
	| ncs-progress__43 |
	| ncs-progress__46 |
	| ncs-progress__48 |
	| ncs-progress__51 |
	| ncs-progress__53 |
	| ncs-progress__56 |
	| ncs-progress__58 |
	| ncs-progress__60 |
	| ncs-progress__63 |
	| ncs-progress__65 |
	| ncs-progress__68 |
	| ncs-progress__70 |
	| ncs-progress__73 |
	| ncs-progress__75 |
	| ncs-progress__78 |
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
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to <Percentage completed> percent
	When I save my progress
	And I make a note of the reference code
	And I navigated to home page
	And I use the reference code to return to my assessment from the Dysac home page
	Then I am at the question where I left off
Examples: 
	| Percentage completed |
	| 2                  |
	| 70                   |
	| 31                   |

@DYSAC
Scenario: TC07 - Current date is displayed on reference code page
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "9" percent
	When I save my progress
	And I view the date on the resultant page
	Then the date is todays date
	When I click the Return to assessment
	Then I am at the question where I left off

@smoke
@DYSAC
Scenario Outline: TC08 - Phone number supplied appears on Check your phone page
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "2" percent
	When I save my progress
	And I choose the "Send reference code in a text message" option of returning to assessment
	When I supply phone number "07585082797"
	When I click the Return to assessment button
	Then I am at the question where I left off

@smoke
@DYSAC
Scenario Outline: TC09 - Phone number field validation
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "2" percent
	And  I get reference code
	And I choose the "Send reference code in a text message" option of returning to assessment
	When I supply phone number ""
	Then validation messages are displayed for the "phone number" field

@smoke
@DYSAC
Scenario: TC10 - Home page reference code field validation
	And I click continue without providing a reference
	Then validation messages are displayed for the "reference code" field

@smoke
@DYSAC
Scenario: TC11 - Email field validation and population
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I select the "Strongly agree" option
	And I proceed with answering questions up to "9" percent
	When I save my progress
	And I choose the "Send reference code in an email" option of returning to assessment	
	When I click Send reference code without providing an email address
	Then validation messages are displayed for the "email address" field
	When I provide email address "chris.mcdaid@education.gov.uk"
	Then I am navigated to the "Check your email" page
	And the email address used is present in the text on the page
	When I click the Back link from the "Check your email" page
	Then I am navigated to the "Email address" page
	When I click the Return to assessment button
	Then I am at the question where I left off
	

@DYSAC
Scenario: TC12 - All question radio button options are usable
	And I click on start skills Assessment	
	And I click on start your Assessment
	Then I am able to select the "Strongly agree" option for the "first" question
	And I am able to select the "Agree" option for the "second" question
	And I am able to select the "It depends" option for the "third" question
	And I am able to select the "Disagree" option for the "fourth" question
	And I am able to select the "Strongly disagree" option for the "fifth" question	

@DYSAC
Scenario Outline: TC13 - Initial and all suggested job categories
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I answer all questions selecting the <Answer option> option
	When I click See results button
	Then the job categories suggestions are <Initial job categories> in number
	And the initial job categories dispalyed are
	| Job category       |
	| Sports and leisure |
	| Creative and media      |
	| Construction and trades |
	When I click See matches to See 7 other career areas that might interest you
	Then the all job categories suggestions are <All job categories> in number
	And all the job categories dispalyed are
	| Job category               |
	| Sports and leisure           |
	| Creative and media           |
	| Construction and trades      |
	| Manufacturing               |	
	| Science and research          |
	| Business and finance           |
	| Emergency and uniform services |
	| Law and legal           |
	| Teaching and education      |
	| Travel and tourism         |
	Examples: 
	| Answer option  | Initial job categories | All job categories |
	| Strongly agree | 3                      | 10                 |

@DYSAC
Scenario Outline: TC14 - Number of answer more questions for each category are correct
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I answer all questions selecting the <Answer option> option
	And I click See results button
	When I click See matches to See 7 other career areas that might interest you
	Then the following are the job categories suggested and their number of answer more questions
	| Job category                   | Number of answer more questions |	
	| Manufacturing                  | 4                               |	
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	| 97               | test question                                                        | It depends        |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                         |
	| you enjoy helping and listening to other people                    |
	| you like dealing with complicated problems or working with numbers |
	| you like to plan things and are well organised                     |
	#And the traits appear in the same order as in the data table above
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category         | Number of answer more questions |
	| Manufacturing        | 4                               |
	| Science and research | 4                               |
	| Travel and tourism   | 2                               |
	| Business and finance | 3                               |
	| Law and legal        | 3                               |
	| Healthcare           | 4                               |
	| Animal care          | 3                               |	
	| Home services        | 3                               |
	| Transport            | 3                               |
	| Delivery and storage | 2                               |
	


@DYSAC
Scenario: TC24 - Real user interaction 2
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	| 97               | test question                                                        | It depends        |
	Then the What you told us section of the Your results page displays the following traits
	| Trait text                                                                                     |
	| you enjoy helping and listening to other people												 |
	| you like dealing with complicated problems or working with numbers		                     |
	| you like to plan things and are well organised                                                 |
	And the following job categories with their corresponding number of answer more questions are displayed
	| Job category                   | Number of answer more questions |
	| Manufacturing        | 4                               |
	| Science and research | 4                               |
	| Travel and tourism   | 2                               |
	| Business and finance | 3                               |
	| Law and legal        | 3                               |
	| Animal care          | 3                               |
	| Healthcare           | 4                               |
	| Delivery and storage  | 2                               |
	| Home services        | 3                               |
	| Transport            | 3                               |
	

	@DYSAC
Scenario Outline: TC25 - Filtering questions links and functionality works as expected
	And I click on start skills Assessment	
	And I click on start your Assessment
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
	When I click "Answer 5 more questions" link
	And Answer "Yes" to the filtering question
	And I click "Back to previous question" link
	Then My last answer is shown
	When I click "Back to results" link
	Then the following are the job categories suggested and their number of answer more questions
		| Job category                   | Number of answer more questions |
		| Sports and leisure             | 2                               |
		| Creative and media             | 4                              |
		| Construction and trades        | 4                               |		
		| Manufacturing                  | 4                               |
		| Science and research           | 3                               |
		| Business and finance           | 2                               |
		| Emergency and uniform services | 2                               |
		| Law and legal                  | 2                               |
		| Teaching and education         | 2                               |
		| Travel and tourism             | 1                               |
	When I click "Answer 3 more questions" link
	Then I should not see my previously answered question
Examples:
	| Answer option  |
	| Strongly agree |


		@DYSAC
Scenario Outline: TC26 - Filtering questions links and job roles
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I answer all questions selecting the <Answer option> option
	And I click See results button
	When I click See matches to See 7 other career areas that might interest you
	When I click "Answer 3 more questions" link
	And Answer "Yes" to the filtering question
	And I click See results button
	Then the following are the job roles suggested
	| Job role                   |
	| Leisure centre assistant             |
	| Outdoor activities instructor                  |
	| PE teacher             |
	| Sport and exercise psychologist        |
	| Lifeguard           |
	| Football referee           |
	| Cinema or theatre attendant |
	| Tourist information centre assistant                |
	| Sports development officer         |
	| Performance sports scientist             |
	| Racehorse trainer name change test 123             |
	| Events manager             |
	
	
Examples:
	| Answer option  |
	| Strongly agree |


	
		@DYSAC
Scenario Outline: TC27 - Filtering questions links and no job roles
	And I click on start skills Assessment	
	And I click on start your Assessment
	And I answer all questions selecting the <Answer option> option
	And I click See results button
	When I click See matches to See 7 other career areas that might interest you
	When I click "Answer 3 more questions" link
	And Answer "No" to the filtering question
	And I click See results button
	Then No job roles should be suggested
	
	
Examples:
	| Answer option  |
	| Strongly agree |


	