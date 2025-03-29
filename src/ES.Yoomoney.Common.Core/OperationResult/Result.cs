namespace ES.Yoomoney.Common.Core.OperationResult;

public class Result
{
    public string? Error { get; init; }

    public bool IsSuccess => string.IsNullOrEmpty(Error);

    public virtual bool IsError => !IsSuccess;

    protected Result(string error)
    {
        Error = error;
    }

    protected Result()
    {
    }

    public static Result Fail(string error) => new(error);

    public static Result Success() => new();
}