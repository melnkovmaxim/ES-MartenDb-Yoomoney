namespace ES.Yoomoney.Infrastructure.Workers.Options;

internal sealed class BackgroundWorkerOptions
{
    public const string Section = nameof(BackgroundWorkerOptions);

    public TimeSpan FetchPaymentPeriod { get; init; }
}