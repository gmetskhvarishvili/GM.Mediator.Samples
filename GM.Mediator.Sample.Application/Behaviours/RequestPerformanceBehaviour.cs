using System.Diagnostics;
using GM.Mediator.Contracts;
using Microsoft.Extensions.Logging;

namespace GM.Mediator.Sample.Application.Behaviours;

/// <summary>
/// Measures how long a request takes and logs a warning when it exceeds the threshold.
/// A simple example of a cross-cutting concern implemented as a pipeline behaviour.
/// </summary>
public class RequestPerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const long SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();

        var response = await next();

        timer.Stop();

        if (timer.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            logger.LogWarning("Long running request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, timer.ElapsedMilliseconds, request);
        }

        return response;
    }
}
