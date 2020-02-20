using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    [ExcludeFromCodeCoverage]
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<AnswerDetail, AnswerDetailsViewModel>();

            CreateMap<FilterAssessmentResult, FilterAssessmentResultViewModel>();

            CreateMap<GetResultsResponse, ResultsIndexResponseViewModel>();

            CreateMap<GetQuestionResponse, QuestionGetResponseViewModel>()
                .ForMember(d => d.Answer, s => s.MapFrom(a => a.RecordedAnswer));

            CreateMap<GetQuestionResponse, FilterQuestionIndexResponseViewModel>();

            CreateMap<JobCategoryResult, JobCategoryResultViewModel>();

            CreateMap<JobProfileResult, JobProfileResultViewModel>();

            CreateMap<TraitValue, TraitValueViewModel>();
        }
    }
}
