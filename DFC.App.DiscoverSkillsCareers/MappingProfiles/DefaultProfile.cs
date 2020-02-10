using AutoMapper;
using DFC.App.DiscoverSkillsCareers.Models.Result;
using DFC.App.DiscoverSkillsCareers.ViewModels;

namespace DFC.App.DiscoverSkillsCareers.MappingProfiles
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<FilterAssessmentResult, FilterAssessmentResultViewModel>();

            CreateMap<GetResultsResponse, ResultIndexResponseViewModel>();

            CreateMap<JobCategoryResult, JobCategoryResultViewModel>();

            CreateMap<JobProfileResult, JobProfileResultViewModel>();

            CreateMap<TraitValue, TraitValueViewModel>();
        }
    }
}
