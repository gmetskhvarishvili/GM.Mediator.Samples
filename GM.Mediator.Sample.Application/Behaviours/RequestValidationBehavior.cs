using FluentValidation;
using GM.Mediator.Contracts;
using ValidationException = GM.Mediator.Sample.Application.Exceptions.ValidationException;

namespace GM.Mediator.Sample.Application.Behaviours;

/// <summary>
/// Runs every registered FluentValidation validator for the request before the handler.
/// If any fail, a <see cref="ValidationException"/> is thrown (mapped to HTTP 400 by the API).
/// </summary>
public class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
