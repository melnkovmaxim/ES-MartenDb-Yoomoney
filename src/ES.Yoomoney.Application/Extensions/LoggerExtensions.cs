using Microsoft.Extensions.Logging;

namespace ES.Yoomoney.Application.Extensions;

internal static class LoggerExtensions
{
    public static IDisposable? BeginAccountScope<T>(this ILogger<T> logger, Guid accountId)
    {
        var loggerMetadata = new Dictionary<string, string>()
        {
            [nameof(accountId)] = accountId.ToString()
        };

        return logger.BeginScope(loggerMetadata);
    }
}