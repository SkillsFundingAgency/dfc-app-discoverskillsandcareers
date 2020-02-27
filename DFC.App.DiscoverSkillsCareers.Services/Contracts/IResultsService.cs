using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Contracts
{
    public interface IResultsService<T>
        where T : IAssessmentType, new()
    {
        Task<GetResultsResponse> GetResults(string jobCategory = null);

        Task<FilterAssessmentResponse> FilterAssessment(string jobCategory);

        Task<bool> IsAssessmentComplete();

        Task<GetQuestionResponse> GetFilterQuestion(string jobCategoryName, int questionNumber);

        Task<PostAnswerResponse> AnswerFilterQuestion(int questionNumberReal, string answer);
    }
}