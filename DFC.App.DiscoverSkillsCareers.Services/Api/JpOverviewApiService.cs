using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class JpOverviewApiService : IJpOverviewAPIService
    {
        private readonly HttpClient httpClient;

        public JpOverviewApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public IEnumerable<string> GetOverviewsForProfiles(IEnumerable<string> jobProfileNames)
        {
            var dummyHTML = "<header class='job-profile-hero'> <div class='govuk-width-container'> <div class='govuk-grid-row'> <div class='govuk-grid-column-two-thirds'> <h1> Bailiff</h1> <h2 class='govuk-!-margin-top-5 govuk-!-margin-bottom-3'><span class='sr-hidden'>Alternative titles for this job include </span>Enforcement agent, enforcement officer, High Court enforcement officer</h2> <p>Bailiffs visit properties to collect debts, serve court documents or give notices or summons.</p> </div> </div> <div class='govuk-grid-row'> <div id='Salary' class='column-40 job-profile-heroblock'> <h2> Average salary <span>(a year)</span> </h2> <div class='job-profile-salary job-profile-heroblock-content'> <p class='dfc-code-jpsstarter'>&#xA3;18,000 <span>Starter</span></p> <i class='sr-hidden'>to</i> <p class='dfc-code-jpsexperienced'>&#xA3;40,000 <span>Experienced</span></p> </div> </div> <div id='WorkingHours' class='column-30 job-profile-heroblock'> <h2>Typical hours <span>(a week)</span></h2> <div class='job-profile-hours job-profile-heroblock-content'> <p class='dfc-code-jphours'> 35 to 40 <span class='dfc-code-jpwhoursdetail'>a week</span> </p> </div> </div> <div id='WorkingHoursPatterns' class='column-30 job-profile-heroblock'> <h2> You could work </h2> <div class='job-profile-pattern job-profile-heroblock-content'> <p class='dfc-code-jpwpattern'> evenings / weekends / bank holidays <span class='dfc-code-jpwpatterndetail'>on shifts</span> </p> </div> </div> </div> </div> </header>";

            yield return dummyHTML;
            yield return dummyHTML;
            yield return dummyHTML;
            yield return dummyHTML;
        }
    }
}
