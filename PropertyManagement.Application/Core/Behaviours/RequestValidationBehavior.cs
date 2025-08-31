using FluentValidation;
using MediatR;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Core.Behaviours;

public class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(request, cancellationToken)));

        Error[] errors = validationResults
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailure => validationFailure is not null)
            .Select(failure => new Error(failure.PropertyName, failure.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length > 0) return CreateValidationResult<TResponse>(errors);

        return await next();
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : Result
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (ValidationResult.WithErrors(errors) as TResult)!;
        }

        object validationResult = typeof(ValidationResult<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
            .GetMethod(nameof(ValidationResult.WithErrors))!
            .Invoke(null, new object?[] { errors })!;

        return (TResult)validationResult;
    }
}