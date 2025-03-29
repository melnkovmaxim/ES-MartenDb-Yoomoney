using Microsoft.Extensions.Logging;

namespace ES.Yoomoney.Application.Features.Commands;

internal static partial class CreateInvoiceCommandLog
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "New account created")]
    public static partial void AccountCreated(this ILogger logger);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Invoice created InvoiceId = {invoiceId}, Amount = {amount}, ConfirmationUrl = {confirmationUrl}")]
    public static partial void InvoiceCreated(
        this ILogger logger,
        Guid invoiceId,
        decimal amount,
        Uri confirmationUrl);
}