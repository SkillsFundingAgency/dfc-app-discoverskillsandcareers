﻿using DFC.App.DiscoverSkillsCareers.Models;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IAssessmentApiService
    {
        Task<NewSessionResponse> NewSession(string assessmentType);

        Task<GetQuestionResponse> GetQuestion(string sessionId, string assessment, int questionNumber);

        Task<PostAnswerResponse> AnswerQuestion(string sessionId, PostAnswerRequest postAnswerRequest);
    }
}