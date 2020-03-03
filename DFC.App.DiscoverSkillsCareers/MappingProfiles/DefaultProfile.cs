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
            CreateMap<AnswerDetail, AnswerDetailsViewModel>()
                .ForMember(d => d.AnsweredOn, x => x.MapFrom(s => s.AnsweredDt))
                .ForMember(d => d.Answer, x => x.MapFrom(s => s.SelectedOption))
                ;

            CreateMap<FilterAssessmentResult, FilterAssessmentResultViewModel>();

            CreateMap<GetResultsResponse, ResultsIndexResponseViewModel>();

            CreateMap<GetQuestionResponse, QuestionGetResponseViewModel>()
                .ForMember(d => d.Answer, s => s.MapFrom(a => a.RecordedAnswer))
                .ForMember(d => d.Started, s => s.MapFrom(a => a.StartedDt))
                .ForMember(d => d.PercentageComplete, s => s.MapFrom(a => a.PercentComplete))
                .ForMember(d => d.AvailableQuestionsCount, s => s.MapFrom(a => a.MaxQuestionsCount))
                ;

            CreateMap<GetQuestionResponse, FilterQuestionIndexResponseViewModel>();

            CreateMap<JobCategoryResult, JobCategoryResultViewModel>();

            CreateMap<JobProfileResult, JobProfileResultViewModel>()
                .ForMember(d => d.JobCategoryName, s => s.MapFrom(a => a.JobCategory));

            CreateMap<TraitValue, TraitValueViewModel>();
        }
    }
}
