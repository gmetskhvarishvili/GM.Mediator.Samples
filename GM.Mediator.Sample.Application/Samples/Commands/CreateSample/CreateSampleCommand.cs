using GM.Mediator.Contracts;

namespace GM.Mediator.Sample.Application.Samples.Commands.CreateSample;

public class CreateSampleCommand : IRequest<string>
{
    public required string Test { get; set; }
}

public class CreateSampleCommandHandler : IRequestHandler<CreateSampleCommand, string>
{
    public async Task<string> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        return $"Property {request.Test} passed successfully!";
    }
}