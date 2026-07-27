using System.Text.Json;
using FluentValidation.Results;

namespace GM.Mediator.Sample.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("Validation Error")
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(string error)
        : base(error)
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(List<ValidationFailure> failures) : base(ModifyMessage(failures))
    {
        Failures = new Dictionary<string, string[]>();

        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var propertyName in propertyNames)
        {
            var propertyFailures = failures
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();

            Failures.Add(propertyName, propertyFailures);
        }
    }

    private static string ModifyMessage(List<ValidationFailure> failures)
    {
        var myFailures = new Dictionary<string, string[]>();

        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var propertyName in propertyNames)
        {
            var propertyFailures = failures
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();

            myFailures.Add(propertyName, propertyFailures);
        }

        
        return JsonSerializer.Serialize(myFailures, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public IDictionary<string, string[]> Failures { get; }
}