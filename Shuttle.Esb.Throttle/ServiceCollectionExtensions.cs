using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Throttle
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddThrottle(this IServiceCollection services, Action<ThrottleBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var throttleBuilder = new ThrottleBuilder(services);

            builder?.Invoke(throttleBuilder);

            services.TryAddSingleton<ThrottleHostedService, ThrottleHostedService>();
            services.TryAddSingleton<ThrottleObserver, ThrottleObserver>();
            services.TryAddSingleton<IThrottlePolicy, ThrottlePolicy>();

            services.AddOptions<ThrottleOptions>().Configure(options =>
            {
                options.AbortCycleCount = throttleBuilder.Options.AbortCycleCount;
                options.CpuUsagePercentage = throttleBuilder.Options.CpuUsagePercentage;
                options.DurationToSleepOnAbort = throttleBuilder.Options.DurationToSleepOnAbort;
            });

            services.AddPipelineModule<ThrottleHostedService>();

            return services;
        }

    }
}