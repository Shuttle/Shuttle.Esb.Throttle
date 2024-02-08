using System;
using System.Collections.Generic;

namespace Shuttle.Esb.Throttle
{
    public class ThrottleOptions
    {
        public const string SectionName = "Shuttle:Modules:Throttle";

        public int CpuUsagePercentage { get; set; } = 65;
        public int AbortCycleCount { get; set; } = 5;
        public List<TimeSpan> DurationToSleepOnAbort { get; set; } = new List<TimeSpan> { TimeSpan.FromSeconds(1) };
    }
}