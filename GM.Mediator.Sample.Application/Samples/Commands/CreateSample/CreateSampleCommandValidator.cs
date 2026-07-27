using FluentValidation;

namespace GM.Mediator.Sample.Application.Samples.Commands.CreateSample;

/// <summary>
/// Validates <see cref="CreateSampleCommand"/>. Discovered automatically by
/// <c>AddValidatorsFromAssembly</c> and executed by the request validation behaviour.
/// </summary>
public class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
{
    public CreateSampleCommandValidator()
    {
        RuleFor(command => command.Test)
            .NotEmpty().WithMessage("'Test' must not be empty.")
            .MaximumLength(100).WithMessage("'Test' must not exceed 100 characters.");
    }
}
