using System.Diagnostics.CodeAnalysis;

namespace ES.Yoomoney.Common.Core.FunctionalResult;

public class Result<TValue> : Result
{
    public TValue? Value { get; init; }

    [MemberNotNullWhen(false, nameof(Value))]
    public override bool IsError => !IsSuccess;

    protected Result(string error)
        : base(error)
    {
        Value = default;
    }

    protected Result(TValue value)
    {
        Value = value;
    }

    public static new Result<TValue> Fail(string error) => new(error);

    public static Result<TValue> Success(TValue value) => new(value);
}