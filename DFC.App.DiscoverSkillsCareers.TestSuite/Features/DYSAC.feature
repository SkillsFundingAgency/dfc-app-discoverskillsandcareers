@webtest
Feature: DYSACUserActions


@DYSAC
Scenario: Starting Assessment loads the first questions
	Given I load the DYSAC page
	When I click on Assessment
	Then The first question is displayed; I am comfortable telling people what they need to do

@DYSAC
Scenario: Starting Assessment loads the questions and show the percentage completion. Also displays the results
	Given I load the DYSAC page
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
Scenario: Display error message when moving to next question without selecting an option 
	Given I load the DYSAC page
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
Scenario: Clicking back link takes to the previous question and updates the percentage completion 
	Given I load the DYSAC page
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
Scenario: Saving progress and selecting reference code to return to the assessment displays reference code 
	Given I load the DYSAC page
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


