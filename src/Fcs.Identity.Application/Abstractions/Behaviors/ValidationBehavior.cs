using Fcs.Identity.Domain.Shared.Results;
using Fcs.Identity.Resources.Messages;
using FluentValidation;
using MediatR;

namespace Fcs.Identity.Application.Abstractions.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToArray();

        if (failures.Length == 0)
        {
            return await next(cancellationToken);
        }

        var message = string.Join("; ", failures.Select(failure => failure.ErrorMessage));
        var error = Error.Validation(IdentityErrorCodes.ValidationFailed, message);

        return CreateValidationResult<TResponse>(error);
    }

    private static TValidationResponse CreateValidationResult<TValidationResponse>(Error error)
    {
        var responseType = typeof(TValidationResponse);

        if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(Result<>))
        {
            throw new ValidationException(error.Message);
        }

        var valueType = responseType.GetGenericArguments()[0];
        var failureMethod = typeof(Result<>)
            .MakeGenericType(valueType)
            .GetMethod(nameof(Result<object>.Failure), [typeof(Error)]);

        return (TValidationResponse)failureMethod!.Invoke(null, [error])!;
    }
}
