using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Esb.Throttle;
// https://github.com/jackowild/CpuUsagePercentageDotNetCoreExample

public class CpuUsage : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(500);
    private readonly Task _task;

    public CpuUsage()
    {
        _task = Task.Run(() =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var startTime = DateTimeOffset.UtcNow;
                var startTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;

                try
                {
                    Task.Delay(_interval).Wait(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                var endTime = DateTimeOffset.UtcNow;
                var endTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;

                var cpuTotalProcessorTime = (endTotalProcessorTime - startTotalProcessorTime).TotalMilliseconds;
                var totalTimePassed = (endTime - startTime).TotalMilliseconds;

                Percentage = cpuTotalProcessorTime / (Environment.ProcessorCount * totalTimePassed) * 100;
            }
        });
    }

    public double Percentage { get; private set; }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _task.Wait(_interval.Add(_interval));
        _task.Dispose();
    }
}