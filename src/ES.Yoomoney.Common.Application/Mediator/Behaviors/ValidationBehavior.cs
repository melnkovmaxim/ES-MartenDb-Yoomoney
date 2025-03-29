using ES.Yoomoney.Common.Core.FunctionalResult;

using FluentValidation;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var errors = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .Select(x => new OperationError(x.PropertyName, x.ErrorMessage))
            .ToArray();

        if (errors.Length != 0)
        {
            return CreateValidationResult<TResponse>(errors);
        }

        return await next();
    }

    private static TResult CreateValidationResult<TResult>(IReadOnlyCollection<OperationError> errors)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (ApplicationValidationResult.WithErrors(errors) as TResult)!;
        }

        var validationResult = typeof(ApplicationValidationResult<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
            .GetMethod(nameof(ApplicationValidationResult.WithErrors))!
            .Invoke(obj: null, [errors])!;

        return (TResult)validationResult;
    }
}