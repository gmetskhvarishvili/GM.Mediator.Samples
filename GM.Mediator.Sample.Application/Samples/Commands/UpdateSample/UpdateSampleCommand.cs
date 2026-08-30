using GM.Mediator.Contracts;

namespace GM.Mediator.Sample.Application.Samples.Commands.UpdateSample;

public sealed record UpdateSampleCommand : IRequest
{
    public required string Test { get; init; }
}

public sealed class UpdateSampleCommandHandler : IRequestHandler<UpdateSampleCommand>
{
    public Task Handle(UpdateSampleCommand request, CancellationToken cancellationToken)
    {
        // Pretend to update something here.
        return Task.CompletedTask;
    }
}
