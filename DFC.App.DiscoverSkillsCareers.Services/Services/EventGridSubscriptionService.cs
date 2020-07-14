using DFC.App.DiscoverSkillsCareers.Models.ClientOptions;
using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.App.DiscoverSkillsCareers.Services.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Services.Api
{
    public class EventGridSubscriptionService : IEventGridSubscriptionService
    {
        private readonly ILogger<EventGridSubscriptionService> logger;
        private readonly EventGridSubscriptionClientOptions eventGridSubscriptionClientOptions;
        private readonly EventGridSubscriptionModel eventGridSubscriptionModel;
        private readonly IApiDataProcessorService apiDataProcessorService;
        private readonly HttpClient httpClient;

        public EventGridSubscriptionService(
            ILogger<EventGridSubscriptionService> logger,
            EventGridSubscriptionClientOptions eventGridSubscriptionClientOptions,
            EventGridSubscriptionModel eventGridSubscriptionModel,
            IApiDataProcessorService apiDataProcessorService,
            HttpClient httpClient)
        {
            this.logger = logger;
            this.eventGridSubscriptionClientOptions = eventGridSubscriptionClientOptions;
            this.eventGridSubscriptionModel = eventGridSubscriptionModel;
            this.apiDataProcessorService = apiDataProcessorService;
            this.httpClient = httpClient;
        }

        public async Task<HttpStatusCode> CreateAsync()
        {
            if (string.IsNullOrWhiteSpace(eventGridSubscriptionClientOptions.BaseAddress?.ToString()))
            {
                logger.LogWarning($"{nameof(CreateAsync)} skipping Event Grid subscription create for: {eventGridSubscriptionModel.Name}, due to no BaseAddress");

                return HttpStatusCode.Continue;
            }

            var url = new Uri($"{eventGridSubscriptionClientOptions.BaseAddress}{eventGridSubscriptionClientOptions.Endpoint}", UriKind.Absolute);

           // var statusCode = await apiDataProcessorService.PostAsync(httpClient, url, eventGridSubscriptionModel).ConfigureAwait(false);
            var statusCode = HttpStatusCode.Created;

            if (statusCode == HttpStatusCode.Created)
            {
                logger.LogInformation($"{nameof(CreateAsync)} has created an Event Grid subscription for: {eventGridSubscriptionModel.Name}");
            }
            else
            {
                logger.LogWarning($"{nameof(CreateAsync)} has not created an Event Grid subscription for: {eventGridSubscriptionModel.Name}: status code :{statusCode}");
            }

            return statusCode;
        }

        public async Task<HttpStatusCode> DeleteAsync()
        {
            if (string.IsNullOrWhiteSpace(eventGridSubscriptionClientOptions.BaseAddress?.ToString()))
            {
                logger.LogWarning($"{nameof(DeleteAsync)} skipping Event Grid subscription delete for: {eventGridSubscriptionModel.Name}, due to no BaseAddress");

                return HttpStatusCode.Continue;
            }

            var url = new Uri($"{eventGridSubscriptionClientOptions.BaseAddress}{eventGridSubscriptionClientOptions.Endpoint}{eventGridSubscriptionModel.Name}", UriKind.Absolute);

            var statusCode = HttpStatusCode.OK;
            //var statusCode = await apiDataProcessorService.DeleteAsync(httpClient, url).ConfigureAwait(false);

            if (statusCode == HttpStatusCode.OK)
            {
                logger.LogInformation($"{nameof(DeleteAsync)} has deleted an Event Grid subscription for: {eventGridSubscriptionModel.Name}");
            }
            else
            {
                logger.LogWarning($"{nameof(DeleteAsync)} has not deleted an Event Grid subscription for: {eventGridSubscriptionModel.Name}: status code :{statusCode}");
            }

            return statusCode;
        }
    }
}