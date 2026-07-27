using FluentValidation;
using GM.Mediator.Sample.Application.Behaviours;
using GM.Mediator.Sample.Application.Samples.Commands.CreateSample;
using Xunit;
using ValidationException = GM.Mediator.Sample.Application.Exceptions.ValidationException;

namespace GM.Mediator.Sample.Tests.Application;

public class RequestValidationBehaviorTests
{
    private static RequestValidationBehavior<CreateSampleCommand, string> BehaviorWith(
        params IValidator<CreateSampleCommand>[] validators)
        => new(validators);

    [Fact]
    public async Task Calls_next_when_the_request_is_valid()
    {
        var behavior = BehaviorWith(new CreateSampleCommandValidator());
        var nextCalled = false;

        var result = await behavior.Handle(
            new CreateSampleCommand { Test = "ok" },
            () => { nextCalled = true; return Task.FromResult("done"); },
            CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("done", result);
    }

    [Fact]
    public async Task Throws_and_short_circuits_when_the_request_is_invalid()
    {
        var behavior = BehaviorWith(new CreateSampleCommandValidator());
        var nextCalled = false;

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(
                new CreateSampleCommand { Test = "" },
                () => { nextCalled = true; return Task.FromResult("done"); },
                CancellationToken.None));

        Assert.False(nextCalled);
        Assert.Contains(nameof(CreateSampleCommand.Test), exception.Failures.Keys);
    }

    [Fact]
    public async Task Calls_next_when_no_validators_are_registered()
    {
        var behavior = BehaviorWith();

        var result = await behavior.Handle(
            new CreateSampleCommand { Test = "" },
            () => Task.FromResult("done"),
            CancellationToken.None);

        Assert.Equal("done", result);
    }
}
