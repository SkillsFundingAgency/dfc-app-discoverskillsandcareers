using DFC.App.DiscoverSkillsCareers.Models.Common;
using DFC.Content.Pkg.Netcore.Data.Models.PollyOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Mime;

namespace DFC.App.DiscoverSkillsCareers.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolicies(
           this IServiceCollection services,
           IPolicyRegistry<string> policyRegistry,
           string keyPrefix,
           PolicyOptions policyOptions)
        {
            if (policyOptions == null)
            {
                throw new ArgumentNullException(nameof(policyOptions));
            }

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpRetry)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpCircuitBreaker)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }
    }
}
