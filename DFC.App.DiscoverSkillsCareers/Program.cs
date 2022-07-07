using DFC.Compui.Telemetry.HostExtensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.DiscoverSkillsCareers
{
    [ExcludeFromCodeCoverage]
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args);
            webHost.Build().AddApplicationTelemetryInitializer().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((webHostBuilderContext, loggingBuilder) =>
                {
                    // This filter is for app insights only
                    loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Trace);
                })
                .UseStartup<Startup>();
    }
}
