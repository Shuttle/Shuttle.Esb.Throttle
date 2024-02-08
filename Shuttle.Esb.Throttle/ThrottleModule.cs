using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Throttle
{
    public class ThrottleHostedService : IHostedService
    {
        private readonly Type _inboxMessagePipelineType = typeof(InboxMessagePipeline);
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IThrottlePolicy _policy;
        private readonly ThrottleOptions _throttleOptions;

        public ThrottleHostedService(IOptions<ThrottleOptions> throttleOptions, IPipelineFactory pipelineFactory, IThrottlePolicy policy)
        {
            Guard.AgainstNull(throttleOptions, nameof(throttleOptions));
            Guard.AgainstNull(throttleOptions.Value, nameof(throttleOptions.Value));

            _throttleOptions = throttleOptions.Value;
            _pipelineFactory = Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));
            _policy = Guard.AgainstNull(policy, nameof(policy));

            _pipelineFactory.PipelineCreated += PipelineCreated;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _pipelineFactory.PipelineCreated -= PipelineCreated;

            await Task.CompletedTask;
        }

        private void PipelineCreated(object sender, PipelineEventArgs e)
        {
            var pipelineType = e.Pipeline.GetType();

            if (pipelineType != _inboxMessagePipelineType)
            {
                return;
            }

            e.Pipeline.RegisterObserver(new ThrottleObserver(_throttleOptions, _policy, e.Pipeline.State.GetCancellationToken()));
        }
    }
}