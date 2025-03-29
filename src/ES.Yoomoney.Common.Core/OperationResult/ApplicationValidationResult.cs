namespace ES.Yoomoney.Common.Core.OperationResult;

public sealed class ApplicationValidationResult : Result
{
    public IReadOnlyCollection<OperationError> Errors { get; }

    private ApplicationValidationResult(IReadOnlyCollection<OperationError> errors)
        : base("Validation error")
    {
        Errors = errors;
    }

    public static ApplicationValidationResult WithErrors(IReadOnlyCollection<OperationError> errors) => new(errors);
}