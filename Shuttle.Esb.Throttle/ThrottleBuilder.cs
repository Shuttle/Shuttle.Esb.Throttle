using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Throttle;

public class ThrottleBuilder
{
    private ThrottleOptions _throttleOptions = new();

    public ThrottleBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public ThrottleOptions Options
    {
        get => _throttleOptions;
        set => _throttleOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; }
}