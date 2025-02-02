using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Threading;

namespace Shuttle.Esb.Throttle;

public class ThrottleHostedService : IHostedService
{
    private readonly CancellationToken _cancellationToken;
    private readonly Type _inboxMessagePipelineType = typeof(InboxMessagePipeline);
    private readonly IPipelineFactory _pipelineFactory;
    private readonly IThrottlePolicy _policy;
    private readonly ThrottleOptions _throttleOptions;

    public ThrottleHostedService(IOptions<ThrottleOptions> throttleOptions, IPipelineFactory pipelineFactory, IThrottlePolicy policy, ICancellationTokenSource cancellationTokenSource)
    {
        _throttleOptions = Guard.AgainstNull(Guard.AgainstNull(throttleOptions).Value);
        _pipelineFactory = Guard.AgainstNull(pipelineFactory);
        _policy = Guard.AgainstNull(policy);

        _cancellationToken = Guard.AgainstNull(cancellationTokenSource).Get().Token;

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

    private void PipelineCreated(object? sender, PipelineEventArgs e)
    {
        var pipelineType = e.Pipeline.GetType();

        if (pipelineType != _inboxMessagePipelineType)
        {
            return;
        }

        e.Pipeline.AddObserver(new ThrottleObserver(_throttleOptions, _policy, _cancellationToken));
    }
}