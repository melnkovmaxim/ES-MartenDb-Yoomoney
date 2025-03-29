namespace ES.Yoomoney.Common.Core.FunctionalEither;

public readonly struct Either<TLeft, TRight>
{
    private readonly TLeft? _left;
    private readonly TRight? _right;
    public bool IsLeft { get; }
    public bool IsRight => !IsLeft;

    private Either(TLeft left)
    {
        _left = left;
        _right = default;
        IsLeft = true;
    }

    private Either(TRight right)
    {
        _right = right;
        _left = default;
        IsLeft = false;
    }

    public static Either<TLeft, TRight> Left(TLeft left) => new(left);
    public static Either<TLeft, TRight> Right(TRight right) => new(right);

    public TResult Match<TResult>(Func<TLeft, TResult> onLeft, Func<TRight, TResult> onRight)
        => IsLeft ? onLeft(_left!) : onRight(_right!);
}
