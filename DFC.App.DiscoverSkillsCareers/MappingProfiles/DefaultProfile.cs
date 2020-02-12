using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Assessment;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<FilterAssessmentResult, FilterAssessmentResultViewModel>();

            CreateMap<GetResultsResponse, ResultsIndexResponseViewModel>();

            CreateMap<JobCategoryResult, JobCategoryResultViewModel>();

            CreateMap<JobProfileResult, JobProfileResultViewModel>();

            CreateMap<TraitValue, TraitValueViewModel>();

            CreateMap<GetQuestionResponse, QuestionGetResponseViewModel>()
                .ForMember(d => d.PercentageComplete, s => s.MapFrom(a => a.PercentComplete))
                .ForMember(d => d.Answer, s => s.MapFrom(a => a.RecordedAnswer));

            CreateMap<GetQuestionResponse, FilterQuestionIndexResponseViewModel>();
        }
    }
}
