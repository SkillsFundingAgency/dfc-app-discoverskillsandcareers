using AutoMapper;
using DFC.App.DiscoverSkillsCareers.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Razor.Templating.Core;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.GraphQl
{
    public class GraphQlService : IGraphQlService
    {
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IMapper mapper;

        public GraphQlService(ISharedContentRedisInterface sharedContentRedisInterface, IMapper mapper)
        {
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.mapper = mapper;
        }

        public async Task<JobProfileViewModel> GetJobProfileAsync(string jobProfile)
        {
            var response = await sharedContentRedisInterface.GetDataAsync<JobProfileOverviewResponse>($"JobProfileOverview/{jobProfile}")
                ?? new JobProfileOverviewResponse();

            var mappedResponse = mapper.Map<JobProfileViewModel>(response);

            /*mappedResponse.SalaryStarterPerYear = ConvertCurrencyToGBP(mappedResponse.SalaryStarterPerYear);
            mappedResponse.SalaryExperiencedPerYear = ConvertCurrencyToGBP(mappedResponse.SalaryExperiencedPerYear);*/
            //mappedResponse.Html = OverviewHTMLBuilder(mappedResponse);
            mappedResponse.Html = await RazorTemplateEngine.RenderAsync("~/Views/Results/JobProfileOverview.cshtml");

            return mappedResponse;
        }

        /*public async string OverviewHTMLBuilder(JobProfileViewModel response)
        {
            var jobProfile = response;
            var urlName = jobProfile.UrlName;
            var displayText = jobProfile.DisplayText;
            var overview = jobProfile.Overview;
            var salaryStarterPerYear = jobProfile.SalaryStarterPerYear;
            var salaryExperiencedPerYear = jobProfile.SalaryExperiencedPerYear;
            var minimumHours = jobProfile.MinimumHours;
            var maximumHours = jobProfile.MaximumHours;
            var workingHoursDetails = jobProfile.WorkingHoursDetails;
            var workingPattern = jobProfile.WorkingPattern;
            var workingPatternDetails = jobProfile.WorkingPatternDetails;

            return $"<div class=\"app-long-results__item\">\r\n    <div class=\"result-description\">\r\n        <h3 class=\"govuk-heading-s\">\r\n            <a href='/job-profiles/{urlName}'>{displayText}</a>\r\n        </h3>\r\n        <p>{overview}</p>\r\n    </div>\r\n    <div class=\"result-detail result-detail--salary\">\r\n        <h4 class=\"govuk-heading-s govuk-heading-s--alt\">Average salary</h4>\r\n        <div class=\"embedded-fa-icon\">\r\n            <svg aria-hidden=\"true\" focusable=\"false\" data-prefix=\"fas\" data-icon=\"pound-sign\" role=\"img\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 320 512\" class=\"svg-inline--fa fa-pound-sign fa-w-10 fa-2x\">\r\n                <path fill=\"currentColor\" d=\"M308 352h-45.495c-6.627 0-12 5.373-12 12v50.848H128V288h84c6.627 0 12-5.373 12-12v-40c0-6.627-5.373-12-12-12h-84v-63.556c0-32.266 24.562-57.086 61.792-57.086 23.658 0 45.878 11.505 57.652 18.849 5.151 3.213 11.888 2.051 15.688-2.685l28.493-35.513c4.233-5.276 3.279-13.005-2.119-17.081C273.124 54.56 236.576 32 187.931 32 106.026 32 48 84.742 48 157.961V224H20c-6.627 0-12 5.373-12 12v40c0 6.627 5.373 12 12 12h28v128H12c-6.627 0-12 5.373-12 12v40c0 6.627 5.373 12 12 12h296c6.627 0 12-5.373 12-12V364c0-6.627-5.373-12-12-12z\" class=\"\"></path>\r\n            </svg>\r\n        </div>\r\n        <div class=\"result-meta\">\r\n            <div>\r\n                <p class=\"salary-min\">\r\n                    {salaryStarterPerYear}<span class=\"govuk-visually-hidden\">&nbsp;(starter)</span>\r\n                </p>\r\n                <p class=\"salary-max\">\r\n                    <span class=\"govuk-visually-hidden\">up to &nbsp;</span>\r\n                    {salaryExperiencedPerYear}<span class=\"govuk-visually-hidden\">&nbsp;(experienced)</span>\r\n                </p>\r\n                <div class=\"range-bar\" aria-hidden=\"true\">\r\n                    <hr>\r\n                </div>\r\n                <p class=\"level-min\">Starter</p>\r\n                <p class=\"level-max\">Experienced</p>\r\n            </div>\r\n        </div>\r\n        <div class=\"result-detail\">\r\n            <h4 class=\"govuk-heading-s govuk-heading-s--alt\">Typical hours</h4>\r\n            <div class=\"embedded-fa-icon\">\r\n                <svg aria-hidden=\"true\" focusable=\"false\" data-prefix=\"far\" data-icon=\"calendar-alt\" role=\"img\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 448 512\" class=\"svg-inline--fa fa-calendar-alt fa-w-14 fa-2x\">\r\n                    <path fill=\"currentColor\" d=\"M148 288h-40c-6.6 0-12-5.4-12-12v-40c0-6.6 5.4-12 12-12h40c6.6 0 12 5.4 12 12v40c0 6.6-5.4 12-12 12zm108-12v-40c0-6.6-5.4-12-12-12h-40c-6.6 0-12 5.4-12 12v40c0 6.6 5.4 12 12 12h40c6.6 0 12-5.4 12-12zm96 0v-40c0-6.6-5.4-12-12-12h-40c-6.6 0-12 5.4-12 12v40c0 6.6 5.4 12 12 12h40c6.6 0 12-5.4 12-12zm-96 96v-40c0-6.6-5.4-12-12-12h-40c-6.6 0-12 5.4-12 12v40c0 6.6 5.4 12 12 12h40c6.6 0 12-5.4 12-12zm-96 0v-40c0-6.6-5.4-12-12-12h-40c-6.6 0-12 5.4-12 12v40c0 6.6 5.4 12 12 12h40c6.6 0 12-5.4 12-12zm192 0v-40c0-6.6-5.4-12-12-12h-40c-6.6 0-12 5.4-12 12v40c0 6.6 5.4 12 12 12h40c6.6 0 12-5.4 12-12zm96-260v352c0 26.5-21.5 48-48 48H48c-26.5 0-48-21.5-48-48V112c0-26.5 21.5-48 48-48h48V12c0-6.6 5.4-12 12-12h40c6.6 0 12 5.4 12 12v52h128V12c0-6.6 5.4-12 12-12h40c6.6 0 12 5.4 12 12v52h48c26.5 0 48 21.5 48 48zm-48 346V160H48v298c0 3.3 2.7 6 6 6h340c3.3 0 6-2.7 6-6z\" class=\"\"></path>\r\n                </svg>\r\n            </div>\r\n            <div class=\"result-meta\">\r\n                <p>\r\n                    {minimumHours} to {maximumHours}                <span class=\"dfc-code-jpwhoursdetail\">{workingHoursDetails}</span>\r\n                </p>\r\n            </div>\r\n        </div>\r\n        <div class=\"result-detail\">\r\n            <h4 class=\"govuk-heading-s govuk-heading-s--alt\">You could work</h4>\r\n            <div class=\"embedded-fa-icon\">\r\n                <svg aria-hidden=\"true\" focusable=\"false\" data-prefix=\"far\" data-icon=\"clock\" role=\"img\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 512 512\" class=\"svg-inline--fa fa-clock fa-w-16 fa-2x\">\r\n                    <path fill=\"currentColor\" d=\"M256 8C119 8 8 119 8 256s111 248 248 248 248-111 248-248S393 8 256 8zm0 448c-110.5 0-200-89.5-200-200S145.5 56 256 56s200 89.5 200 200-89.5 200-200 200zm61.8-104.4l-84.9-61.7c-3.1-2.3-4.9-5.9-4.9-9.7V116c0-6.6 5.4-12 12-12h32c6.6 0 12 5.4 12 12v141.7l66.8 48.6c5.4 3.9 6.5 11.4 2.6 16.8L334.6 349c-3.9 5.3-11.4 6.5-16.8 2.6z\" class=\"\"></path>\r\n                </svg>\r\n            </div>\r\n            <div class=\"result-meta\">\r\n                <p>\r\n                    {workingPattern}\r\n                    <span class=\"dfc-code-jpwpatterndetail\">{workingPatternDetails}</span>\r\n                </p>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>";
        }*/

        /*public string ConvertCurrencyToGBP(string amountStr)
        {
            var amount = double.Parse(amountStr);
            return amount.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
        }*/
    }
}
