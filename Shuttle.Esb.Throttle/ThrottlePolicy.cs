using System;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Throttle
{
    public class ThrottlePolicy : IThrottlePolicy, IDisposable
    {
        private readonly ThrottleOptions _throttleOptions;
        private int _abortCount;
        private readonly CpuUsage _cpuUsage;

        public ThrottlePolicy(IOptions<ThrottleOptions> throttleOptions)
        {
            Guard.AgainstNull(throttleOptions, nameof(throttleOptions));
            Guard.AgainstNull(throttleOptions.Value, nameof(throttleOptions.Value));

            _throttleOptions = throttleOptions.Value;

            _cpuUsage = new CpuUsage();
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

        public void Dispose()
        {
            _cpuUsage?.Dispose();
        }
    }
}