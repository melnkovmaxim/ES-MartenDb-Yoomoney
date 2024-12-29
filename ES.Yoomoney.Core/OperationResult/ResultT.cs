namespace ES.Yoomoney.Core.OperationResult;

public sealed class Result<T> : Result
{
    public T Value { get; init; }
    
    private Result(string error) : base(error)
    {
    }

    private Result(T value) : base()
    {
        Value = value;
    }

    public new static Result<T> Fail(string error) => new Result<T>(error);
    public new static Result<T> Success(T value) => new Result<T>(value);
}
