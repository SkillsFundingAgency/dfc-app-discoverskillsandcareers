using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Cosmos;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.DiscoverSkillsCareers.Models
{
    public class CosmosDbAppInsightsRequestHandler : RequestHandler
    {
        private readonly TelemetryClient telemetryClient;

        public CosmosDbAppInsightsRequestHandler(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public override async Task<ResponseMessage> SendAsync(RequestMessage request, CancellationToken cancellationToken)
        {
            using var dependency = telemetryClient.StartOperation<DependencyTelemetry>($"{request.Method} {request.RequestUri.OriginalString}");

            var response = await base.SendAsync(request, cancellationToken);

            var telemetry = dependency.Telemetry;

            // Used to identify Cosmos DB in Application Insights
            telemetry.Type = "Azure DocumentDB";
            telemetry.Data = request.RequestUri.OriginalString;

            request.Properties.TryGetValue("ContainerId", out var test1);

            telemetry.ResultCode = ((int)response.StatusCode).ToString();
            telemetry.Success = response.IsSuccessStatusCode;

            // Send with Metrics
            telemetry.Metrics["RequestCharge"] = response.Headers.RequestCharge;
            telemetry.Metrics["Duration"] = response.Diagnostics.GetClientElapsedTime().TotalSeconds;

            return response;
        }
    }
}
