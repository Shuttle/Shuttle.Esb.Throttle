using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Throttle;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddThrottle(this IServiceCollection services, Action<ThrottleBuilder>? builder = null)
    {
        var throttleBuilder = new ThrottleBuilder(Guard.AgainstNull(services));

        builder?.Invoke(throttleBuilder);

        services.TryAddSingleton<ThrottleHostedService, ThrottleHostedService>();
        services.TryAddSingleton<ThrottleObserver, ThrottleObserver>();
        services.TryAddSingleton<IThrottlePolicy, ThrottlePolicy>();

        services.AddSingleton(Options.Create(throttleBuilder.Options));

        services.AddHostedService<ThrottleHostedService>();

        return services;
    }
}