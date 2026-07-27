using GM.Mediator.Sample.Application.Samples.Commands.CreateSample;
using Xunit;

namespace GM.Mediator.Sample.Tests.Application;

public class CreateSampleTests
{
    [Fact]
    public async Task Handler_returns_success_message_with_the_supplied_value()
    {
        var handler = new CreateSampleCommandHandler();

        var result = await handler.Handle(new CreateSampleCommand { Test = "abc" }, CancellationToken.None);

        Assert.Equal("Property abc passed successfully!", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validator_rejects_empty_values(string value)
    {
        var validator = new CreateSampleCommandValidator();

        var result = validator.Validate(new CreateSampleCommand { Test = value });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateSampleCommand.Test));
    }

    [Fact]
    public void Validator_rejects_values_over_the_length_limit()
    {
        var validator = new CreateSampleCommandValidator();

        var result = validator.Validate(new CreateSampleCommand { Test = new string('x', 101) });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validator_accepts_a_valid_value()
    {
        var validator = new CreateSampleCommandValidator();

        var result = validator.Validate(new CreateSampleCommand { Test = "hello" });

        Assert.True(result.IsValid);
    }
}
