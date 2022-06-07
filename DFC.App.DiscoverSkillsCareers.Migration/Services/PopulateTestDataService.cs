using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.App.DiscoverSkillsCareers.Migration.Contacts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;

namespace DFC.App.DiscoverSkillsCareers.Migration.Services
{
    public class PopulateTestDataService : IMigrationService
    {
        private readonly IDocumentClient destinationDocumentClient;
        private static Random random = new Random();
        
        public PopulateTestDataService(
            IDocumentClient destinationDocumentClient)
        {
            this.destinationDocumentClient = destinationDocumentClient;
        }

        public async Task Start()
        {
            var itemsToInsert = 300000;
            var batchSize = 80;  // 3 at 400, 8 at 1000, 80 at 10,000, 320 at 40,000 - charge is 116 RUs
            var counter = 0;

            while (counter < itemsToInsert)
            {
                var populateTasks = new List<Task>();

                for (int idx = 0, len = batchSize; idx < len; idx++)
                {
                    var assessment = Document.Replace("[id]", RandomString(14)).Replace("[partitionKey]", $"session{counter+idx}");
                    populateTasks.Add(Add(assessment, counter + idx + 1, itemsToInsert));
                }
                
                await Task.WhenAll(populateTasks);
                counter += batchSize;  
            }
        }
        
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();
        }
        
        private async Task Add(string migratedAssessment, int index, int count)
        {
            Log($"Started creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            var start = DateTime.Now;
            
            var resourceResponse = await destinationDocumentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri("DiscoverMySkillsAndCareers", "UserSessions"),
                JObject.Parse(migratedAssessment),
                new RequestOptions());
            
            var charge = resourceResponse.RequestCharge;
            
            Log($"Finished creating assessment {index} of {count} - {DateTime.Now:yyyy-MM-dd hh:mm:ss} - took " +
                $"{(DateTime.Now - start).TotalSeconds} seconds. Charge was {charge} RUs");
        }
        
        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        private string Document =
            @"{
 ""partitionKey"": ""[partitionKey]"",
 ""id"": ""[id]"",
 ""languageCode"": ""en"",
 ""salt"": ""ncs"",
 ""assessmentState"": {
 ""questionSetVersion"": ""short-201901-24"",
 ""currentQuestion"": 40,
 ""maxQuestions"": 40,
 ""recordedAnswers"": [
 {
 ""questionId"": ""short-201901-24-1"",
 ""questionNumber"": 1,
 ""questionText"": ""I am comfortable telling people what they need to do. (Dev Modified)"",
 ""traitCode"": ""LEADER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:01.4881075Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-2"",
 ""questionNumber"": 2,
 ""questionText"": ""I make decisions quickly"",
 ""traitCode"": ""LEADER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:02.7031997Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-3"",
 ""questionNumber"": 3,
 ""questionText"": ""I like to take control of situations"",
 ""traitCode"": ""LEADER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:04.1328801Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-4"",
 ""questionNumber"": 4,
 ""questionText"": ""I prefer to follow what other people are doing"",
 ""traitCode"": ""LEADER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:04.9940341Z"",
 ""isNegative"": true,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-5"",
 ""questionNumber"": 5,
 ""questionText"": ""I like taking responsibility for other people"",
 ""traitCode"": ""LEADER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:05.9289619Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-6"",
 ""questionNumber"": 6,
 ""questionText"": ""I set myself targets when I have things to do, and usually meet them"",
 ""traitCode"": ""DRIVER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:06.9196111Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-7"",
 ""questionNumber"": 7,
 ""questionText"": ""I like to see things through to the end"",
 ""traitCode"": ""DRIVER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:08.0040701Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-8"",
 ""questionNumber"": 8,
 ""questionText"": ""I think I am a competitive person"",
 ""traitCode"": ""DRIVER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:09.1813247Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-9"",
 ""questionNumber"": 9,
 ""questionText"": ""Doing well in a career motivates me"",
 ""traitCode"": ""DRIVER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:10.0443537Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-10"",
 ""questionNumber"": 10,
 ""questionText"": ""I set myself goals in life"",
 ""traitCode"": ""DRIVER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:10.9650543Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-11"",
 ""questionNumber"": 11,
 ""questionText"": ""I am comfortable talking people around to my way of thinking"",
 ""traitCode"": ""INFLUENCER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:11.9615596Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-12"",
 ""questionNumber"": 12,
 ""questionText"": ""I am good at coming to an agreement with other people"",
 ""traitCode"": ""INFLUENCER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:12.9908442Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-13"",
 ""questionNumber"": 13,
 ""questionText"": ""I am comfortable talking in front of a group of people"",
 ""traitCode"": ""INFLUENCER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:13.9137679Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-14"",
 ""questionNumber"": 14,
 ""questionText"": ""I like meeting new people"",
 ""traitCode"": ""INFLUENCER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:14.8522206Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-15"",
 ""questionNumber"": 15,
 ""questionText"": ""I find it hard to understand other people's point of view"",
 ""traitCode"": ""INFLUENCER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:15.7606256Z"",
 ""isNegative"": true,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-16"",
 ""questionNumber"": 16,
 ""questionText"": ""I like to help other people"",
 ""traitCode"": ""HELPER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:16.9943349Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-17"",
 ""questionNumber"": 17,
 ""questionText"": ""I enjoy working with other people around me"",
 ""traitCode"": ""HELPER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:17.8706128Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-18"",
 ""questionNumber"": 18,
 ""questionText"": ""I want to make things better for people"",
 ""traitCode"": ""HELPER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:18.7348542Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-19"",
 ""questionNumber"": 19,
 ""questionText"": ""I will get involved if I think I can help"",
 ""traitCode"": ""HELPER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:19.6572851Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-20"",
 ""questionNumber"": 20,
 ""questionText"": ""I am comfortable hearing other people's problems"",
 ""traitCode"": ""HELPER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:20.5279386Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-21"",
 ""questionNumber"": 21,
 ""questionText"": ""I like to work out complicated things"",
 ""traitCode"": ""ANALYST"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:22.1402201Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-22"",
 ""questionNumber"": 22,
 ""questionText"": ""I like to get to the centre of the issue"",
 ""traitCode"": ""ANALYST"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:23.366244Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-23"",
 ""questionNumber"": 23,
 ""questionText"": ""I like working with facts"",
 ""traitCode"": ""ANALYST"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:24.1892897Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-24"",
 ""questionNumber"": 24,
 ""questionText"": ""I like working with numbers"",
 ""traitCode"": ""ANALYST"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:25.1540379Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-25"",
 ""questionNumber"": 25,
 ""questionText"": ""I enjoy learning new things"",
 ""traitCode"": ""ANALYST"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:26.1793881Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-26"",
 ""questionNumber"": 26,
 ""questionText"": ""I enjoy coming up with new ways of doing things"",
 ""traitCode"": ""CREATOR"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:27.3603582Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-27"",
 ""questionNumber"": 27,
 ""questionText"": ""I try to think differently to others"",
 ""traitCode"": ""CREATOR"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:28.4838694Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-28"",
 ""questionNumber"": 28,
 ""questionText"": ""I like to use my imagination to create new things"",
 ""traitCode"": ""CREATOR"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:29.3974943Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-29"",
 ""questionNumber"": 29,
 ""questionText"": ""I like to try new things"",
 ""traitCode"": ""CREATOR"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:30.3115086Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-30"",
 ""questionNumber"": 30,
 ""questionText"": ""I enjoy creative activities"",
 ""traitCode"": ""CREATOR"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:31.2611342Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-31"",
 ""questionNumber"": 31,
 ""questionText"": ""I like to focus on details"",
 ""traitCode"": ""ORGANISER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:32.3570638Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-32"",
 ""questionNumber"": 32,
 ""questionText"": ""I plan my day so I can use my time best"",
 ""traitCode"": ""ORGANISER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:33.2882071Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-33"",
 ""questionNumber"": 33,
 ""questionText"": ""I like doing things in a careful order"",
 ""traitCode"": ""ORGANISER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:34.867319Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-34"",
 ""questionNumber"": 34,
 ""questionText"": ""I like to follow rules and processes"",
 ""traitCode"": ""ORGANISER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:36.0633806Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-35"",
 ""questionNumber"": 35,
 ""questionText"": ""I feel restricted when I have to follow a routine"",
 ""traitCode"": ""ORGANISER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:37.0538746Z"",
 ""isNegative"": true,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-36"",
 ""questionNumber"": 36,
 ""questionText"": ""I like to see the results of the work I do"",
 ""traitCode"": ""DOER"",
 ""selectedOption"": 0,
 ""answeredDt"": ""2022-05-24T12:15:37.8843543Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-37"",
 ""questionNumber"": 37,
 ""questionText"": ""I like to get involved in making things"",
 ""traitCode"": ""DOER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:38.7408611Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-38"",
 ""questionNumber"": 38,
 ""questionText"": ""I enjoy getting involved in practical tasks"",
 ""traitCode"": ""DOER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:39.6549385Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-39"",
 ""questionNumber"": 39,
 ""questionText"": ""I like working with my hands or tools"",
 ""traitCode"": ""DOER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:40.5299979Z"",
 ""isNegative"": false,
 ""questionSetVersion"": ""short-201901-24""
 },
 {
 ""questionId"": ""short-201901-24-40"",
 ""questionNumber"": 40,
 ""questionText"": ""I enjoy planning a task more than actually doing it"",
 ""traitCode"": ""DOER"",
 ""selectedOption"": 4,
 ""answeredDt"": ""2022-05-24T12:15:41.4637829Z"",
 ""isNegative"": true,
 ""questionSetVersion"": ""short-201901-24""
 }
 ],
 ""completeDt"": ""2022-05-24T12:15:41.4637973Z""
 },
 ""filteredAssessmentState"": {
 ""jobCategories"": [
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Speaking, Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-46"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Cooperation"",
 ""QuestionId"": ""filtered-default-67-8"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Initiative"",
 ""QuestionId"": ""filtered-default-67-15"",
 ""QuestionNumber"": 3
 },
 {
 ""Skill"": ""Adaptability/Flexibility"",
 ""QuestionId"": ""filtered-default-67-3"",
 ""QuestionNumber"": 4
 },
 {
 ""Skill"": ""Reading Comprehension"",
 ""QuestionId"": ""filtered-default-67-85"",
 ""QuestionNumber"": 5
 }
 ],
 ""jobCategoryName"": ""Creative and media"",
 ""jobCategoryCode"": ""CAM"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Cooperation"",
 ""QuestionId"": ""filtered-default-67-8"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Self Control"",
 ""QuestionId"": ""filtered-default-67-42"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Initiative"",
 ""QuestionId"": ""filtered-default-67-15"",
 ""QuestionNumber"": 3
 },
 {
 ""Skill"": ""Fine Manipulative Abilities"",
 ""QuestionId"": ""filtered-default-67-12"",
 ""QuestionNumber"": 4
 }
 ],
 ""jobCategoryName"": ""Construction and trades"",
 ""jobCategoryCode"": ""CAT"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Self Control"",
 ""QuestionId"": ""filtered-default-67-42"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Speaking, Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-46"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Cooperation"",
 ""QuestionId"": ""filtered-default-67-8"",
 ""QuestionNumber"": 3
 }
 ],
 ""jobCategoryName"": ""Sports and leisure"",
 ""jobCategoryCode"": ""SAL"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Analytical Thinking"",
 ""QuestionId"": ""filtered-default-67-4"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Reading Comprehension"",
 ""QuestionId"": ""filtered-default-67-85"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Persistence"",
 ""QuestionId"": ""filtered-default-67-32"",
 ""QuestionNumber"": 3
 }
 ],
 ""jobCategoryName"": ""Computing, technology and digital"",
 ""jobCategoryCode"": ""CTAD"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Cooperation"",
 ""QuestionId"": ""filtered-default-67-8"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-53"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Self Control"",
 ""QuestionId"": ""filtered-default-67-42"",
 ""QuestionNumber"": 3
 },
 {
 ""Skill"": ""Fine Manipulative Abilities"",
 ""QuestionId"": ""filtered-default-67-12"",
 ""QuestionNumber"": 4
 }
 ],
 ""jobCategoryName"": ""Manufacturing"",
 ""jobCategoryCode"": ""MANUF"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Analytical Thinking"",
 ""QuestionId"": ""filtered-default-67-4"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Quantitative Abilities, Mathematics Knowledge"",
 ""QuestionId"": ""filtered-default-67-38"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Reading Comprehension"",
 ""QuestionId"": ""filtered-default-67-85"",
 ""QuestionNumber"": 3
 },
 {
 ""Skill"": ""Speaking, Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-46"",
 ""QuestionNumber"": 4
 }
 ],
 ""jobCategoryName"": ""Science and research"",
 ""jobCategoryCode"": ""SAR"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Speaking, Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-46"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Cooperation"",
 ""QuestionId"": ""filtered-default-67-8"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Initiative"",
 ""QuestionId"": ""filtered-default-67-15"",
 ""QuestionNumber"": 3
 },
 {
 ""Skill"": ""Reading Comprehension"",
 ""QuestionId"": ""filtered-default-67-85"",
 ""QuestionNumber"": 4
 }
 ],
 ""jobCategoryName"": ""Managerial"",
 ""jobCategoryCode"": ""MANAG"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Speaking, Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-46"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Quantitative Abilities, Mathematics Knowledge"",
 ""QuestionId"": ""filtered-default-67-38"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Reading Comprehension"",
 ""QuestionId"": ""filtered-default-67-85"",
 ""QuestionNumber"": 3
 }
 ],
 ""jobCategoryName"": ""Business and finance"",
 ""jobCategoryCode"": ""BAF"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Speaking, Verbal Abilities"",
 ""QuestionId"": ""filtered-default-67-46"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Stress Tolerance"",
 ""QuestionId"": ""filtered-default-67-47"",
 ""QuestionNumber"": 2
 },
 {
 ""Skill"": ""Self Control"",
 ""QuestionId"": ""filtered-default-67-42"",
 ""QuestionNumber"": 3
 }
 ],
 ""jobCategoryName"": ""Law and legal"",
 ""jobCategoryCode"": ""LAL"",
 ""currentQuestion"": 1
 },
 {
 ""questionSetVersion"": ""filtered-default-67"",
 ""questions"": [
 {
 ""Skill"": ""Cooperation"",
 ""QuestionId"": ""filtered-default-67-8"",
 ""QuestionNumber"": 1
 },
 {
 ""Skill"": ""Self Control"",
 ""QuestionId"": ""filtered-default-67-42"",
 ""QuestionNumber"": 2
 }
 ],
 ""jobCategoryName"": ""Delivery and storage"",
 ""jobCategoryCode"": ""DAS"",
 ""currentQuestion"": 1
 }
 ],
 ""recordedAnswers"": [],
 ""currentFilterAssessmentCode"": null,
 ""completeDt"": null
 },
 ""resultData"": {
 ""traits"": [
 {
 ""traitCode"": ""LEADER"",
 ""traitName"": ""Leader"",
 ""traitText"": ""you like to lead other people and are good at taking control of situations"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""INFLUENCER"",
 ""traitName"": ""Influencer"",
 ""traitText"": ""you are sociable and find it easy to understand people"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""ANALYST"",
 ""traitName"": ""Analyst"",
 ""traitText"": ""you like dealing with complicated problems or working with numbers"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""CREATOR"",
 ""traitName"": ""Creator"",
 ""traitText"": ""you are a creative person and enjoy coming up with new ways of doing things"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""DRIVER"",
 ""traitName"": ""Driver"",
 ""traitText"": ""you are motivated, set yourself personal goals and are comfortable competing with other people"",
 ""totalScore"": 2
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""traitName"": ""Organiser"",
 ""traitText"": ""you like to plan things and are well organised"",
 ""totalScore"": 2
 },
 {
 ""traitCode"": ""DOER"",
 ""traitName"": ""Doer"",
 ""traitText"": ""you are a practical person and enjoy getting things done"",
 ""totalScore"": 2
 }
 ],
 ""jobFamilies"": [
 {
 ""jobFamilyCode"": ""CAM"",
 ""jobFamilyName"": ""Creative and media"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""creative-and-media"",
 ""traitsTotal"": 14,
 ""total"": 14,
 ""normalizedTotal"": 4.666666666666667,
 ""TraitValues"": [
 {
 ""traitCode"": ""CREATOR"",
 ""total"": 6,
 ""normalizedTotal"": 2
 },
 {
 ""traitCode"": ""DOER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""ANALYST"",
 ""total"": 6,
 ""normalizedTotal"": 2
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 5,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""CAT"",
 ""jobFamilyName"": ""Construction and trades"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""construction-and-trades"",
 ""traitsTotal"": 14,
 ""total"": 14,
 ""normalizedTotal"": 4.666666666666667,
 ""TraitValues"": [
 {
 ""traitCode"": ""CREATOR"",
 ""total"": 6,
 ""normalizedTotal"": 2
 },
 {
 ""traitCode"": ""DOER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""ANALYST"",
 ""total"": 6,
 ""normalizedTotal"": 2
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 4,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""SAL"",
 ""jobFamilyName"": ""Sports and leisure"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""sports-and-leisure"",
 ""traitsTotal"": 14,
 ""total"": 14,
 ""normalizedTotal"": 4.666666666666667,
 ""TraitValues"": [
 {
 ""traitCode"": ""CREATOR"",
 ""total"": 6,
 ""normalizedTotal"": 2
 },
 {
 ""traitCode"": ""ANALYST"",
 ""total"": 6,
 ""normalizedTotal"": 2
 },
 {
 ""traitCode"": ""DRIVER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 3,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""CTAD"",
 ""jobFamilyName"": ""Computing, technology and digital"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""computing-technology-and-digital"",
 ""traitsTotal"": 12,
 ""total"": 12,
 ""normalizedTotal"": 6,
 ""TraitValues"": [
 {
 ""traitCode"": ""CREATOR"",
 ""total"": 6,
 ""normalizedTotal"": 3
 },
 {
 ""traitCode"": ""ANALYST"",
 ""total"": 6,
 ""normalizedTotal"": 3
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 3,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""MANUF"",
 ""jobFamilyName"": ""Manufacturing"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""manufacturing"",
 ""traitsTotal"": 10,
 ""total"": 10,
 ""normalizedTotal"": 3.3333333333333335,
 ""TraitValues"": [
 {
 ""traitCode"": ""ANALYST"",
 ""total"": 6,
 ""normalizedTotal"": 2
 },
 {
 ""traitCode"": ""DRIVER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 4,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""SAR"",
 ""jobFamilyName"": ""Science and research"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""science-and-research"",
 ""traitsTotal"": 10,
 ""total"": 10,
 ""normalizedTotal"": 3.3333333333333335,
 ""TraitValues"": [
 {
 ""traitCode"": ""ANALYST"",
 ""total"": 6,
 ""normalizedTotal"": 2
 },
 {
 ""traitCode"": ""DRIVER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 4,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""MANAG"",
 ""jobFamilyName"": ""Managerial"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""managerial"",
 ""traitsTotal"": 8,
 ""total"": 8,
 ""normalizedTotal"": 4,
 ""TraitValues"": [
 {
 ""traitCode"": ""DRIVER"",
 ""total"": 2,
 ""normalizedTotal"": 1
 },
 {
 ""traitCode"": ""LEADER"",
 ""total"": 6,
 ""normalizedTotal"": 3
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 4,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""BAF"",
 ""jobFamilyName"": ""Business and finance"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""business-and-finance"",
 ""traitsTotal"": 6,
 ""total"": 6,
 ""normalizedTotal"": 2,
 ""TraitValues"": [
 {
 ""traitCode"": ""DOER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""DRIVER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 3,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""LAL"",
 ""jobFamilyName"": ""Law and legal"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""law-and-legal"",
 ""traitsTotal"": 6,
 ""total"": 6,
 ""normalizedTotal"": 2,
 ""TraitValues"": [
 {
 ""traitCode"": ""DOER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""DRIVER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""total"": 2,
 ""normalizedTotal"": 0.6666666666666666
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 3,
 ""resultsShown"": false
 },
 {
 ""jobFamilyCode"": ""DAS"",
 ""jobFamilyName"": ""Delivery and storage"",
 ""jobFamilyText"": """",
 ""jobFamilyUrl"": ""delivery-and-storage"",
 ""traitsTotal"": 4,
 ""total"": 4,
 ""normalizedTotal"": 2,
 ""TraitValues"": [
 {
 ""traitCode"": ""DOER"",
 ""total"": 2,
 ""normalizedTotal"": 1
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""total"": 2,
 ""normalizedTotal"": 1
 }
 ],
 ""filterAssessment"": null,
 ""totalQuestions"": 2,
 ""resultsShown"": false
 }
 ],
 ""traitsscores"": [
 {
 ""traitCode"": ""LEADER"",
 ""traitName"": ""Leader"",
 ""traitText"": ""you like to lead other people and are good at taking control of situations"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""INFLUENCER"",
 ""traitName"": ""Influencer"",
 ""traitText"": ""you are sociable and find it easy to understand people"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""ANALYST"",
 ""traitName"": ""Analyst"",
 ""traitText"": ""you like dealing with complicated problems or working with numbers"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""CREATOR"",
 ""traitName"": ""Creator"",
 ""traitText"": ""you are a creative person and enjoy coming up with new ways of doing things"",
 ""totalScore"": 6
 },
 {
 ""traitCode"": ""DRIVER"",
 ""traitName"": ""Driver"",
 ""traitText"": ""you are motivated, set yourself personal goals and are comfortable competing with other people"",
 ""totalScore"": 2
 },
 {
 ""traitCode"": ""ORGANISER"",
 ""traitName"": ""Organiser"",
 ""traitText"": ""you like to plan things and are well organised"",
 ""totalScore"": 2
 },
 {
 ""traitCode"": ""DOER"",
 ""traitName"": ""Doer"",
 ""traitText"": ""you are a practical person and enjoy getting things done"",
 ""totalScore"": 2
 },
 {
 ""traitCode"": ""HELPER"",
 ""traitName"": ""Helper"",
 ""traitText"": ""you enjoy helping and listening to other people"",
 ""totalScore"": -2
 }
 ]
 },
 ""startedDt"": ""2022-05-24T12:14:59.4404715+00:00"",
 ""assessmentType"": ""short"",
 ""lastUpdatedDt"": ""2022-05-24T12:15:42.5504037Z"",
 ""completeDt"": ""2022-05-24T12:15:41.4637973Z"",
 ""_rid"": ""RYgPAJtqh8oDAAAAAAAAAA=="",
 ""_self"": ""dbs/RYgPAA==/colls/RYgPAJtqh8o=/docs/RYgPAJtqh8oDAAAAAAAAAA==/"",
 ""_etag"": ""\""2200ac1f-0000-0d00-0000-628ccc6e0000\"""",
 ""_attachments"": ""attachments/"",
 ""_ts"": 1653394542
}
";
    }
}