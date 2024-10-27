using System;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Throttle;

public class ThrottleObserver : IPipelineObserver<OnPipelineStarting>
{
    private readonly CancellationToken _cancellationToken;
    private readonly IThrottlePolicy _policy;
    private readonly ThrottleOptions _throttleOptions;
    private int _abortCount;

    public ThrottleObserver(ThrottleOptions throttleOptions, IThrottlePolicy policy, CancellationToken cancellationToken)
    {
        _throttleOptions = Guard.AgainstNull(throttleOptions);
        _policy = Guard.AgainstNull(policy);
        _cancellationToken = cancellationToken;
    }

    public async Task ExecuteAsync(IPipelineContext<OnPipelineStarting> pipelineContext)
    {
        if (!_policy.ShouldAbort())
        {
            _abortCount = 0;
            return;
        }

        pipelineContext.Pipeline.Abort();

        var sleep = TimeSpan.FromSeconds(1);

        try
        {
            sleep = _throttleOptions.DurationToSleepOnAbort[_abortCount];
        }
        catch
        {
            // ignore
        }


        try
        {
            await Task.Delay(sleep, _cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }

        _abortCount += _abortCount + 1 < _throttleOptions.DurationToSleepOnAbort.Count ? 1 : 0;
    }
}