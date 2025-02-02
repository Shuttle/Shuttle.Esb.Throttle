using System;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Throttle;

public class ThrottlePolicy : IThrottlePolicy, IDisposable
{
    private readonly CpuUsage _cpuUsage;
    private readonly ThrottleOptions _throttleOptions;
    private int _abortCount;

    public ThrottlePolicy(IOptions<ThrottleOptions> throttleOptions)
    {
        _throttleOptions = Guard.AgainstNull(Guard.AgainstNull(throttleOptions).Value);

        _cpuUsage = new();
    }

    public void Dispose()
    {
        _cpuUsage.Dispose();
    }

    public bool ShouldAbort()
    {
        if (_cpuUsage.Percentage > _throttleOptions.CpuUsagePercentage)
        {
            _abortCount++;

            if (_abortCount > _throttleOptions.AbortCycleCount)
            {
                _abortCount = 0;
            }

            return _abortCount > 0;
        }

        _abortCount = 0;

        return false;
    }
}