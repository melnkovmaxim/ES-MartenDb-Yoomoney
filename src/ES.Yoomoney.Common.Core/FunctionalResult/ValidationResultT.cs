namespace ES.Yoomoney.Common.Core.FunctionalResult;

public sealed class ApplicationValidationResult<TValue> : Result<TValue>
{
    public IReadOnlyCollection<OperationError> Errors { get; }

    private ApplicationValidationResult(IReadOnlyCollection<OperationError> errors)
        : base("Validation error")
    {
        Errors = errors;
    }

    private ApplicationValidationResult(TValue value)
        : base(value)
    {
        Errors = [];
    }

    // ReSharper disable once UnusedMember.Global
    public static ApplicationValidationResult<TValue> WithErrors(IReadOnlyCollection<OperationError> errors) =>
        new(errors);
}