using System;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Throttle
{
    public class ThrottleObserver : IPipelineObserver<OnPipelineStarting>
    {
        private readonly IThrottlePolicy _policy;
        private readonly CancellationToken _cancellationToken;
        private readonly ThrottleOptions _throttleOptions;
        private int _abortCount;

        public ThrottleObserver(ThrottleOptions throttleOptions, IThrottlePolicy policy, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(throttleOptions, nameof(throttleOptions));
            Guard.AgainstNull(policy, nameof(policy));

            _throttleOptions = throttleOptions;
            _cancellationToken = cancellationToken;
            _policy = policy;
        }

        public void Execute(OnPipelineStarting pipelineEvent)
        {
            if (!_policy.ShouldAbort())
            {
                _abortCount = 0;
                return;
            }

            pipelineEvent.Pipeline.Abort();
            var sleep = TimeSpan.FromSeconds(1);

            try
            {
                sleep = _throttleOptions.DurationToSleepOnAbort[_abortCount];
            }
            catch
            {
            }


            try
            {
                Task.Delay(sleep, _cancellationToken).Wait(_cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }

            _abortCount += _abortCount + 1 < _throttleOptions.DurationToSleepOnAbort.Count ? 1 : 0;
        }
    }
}