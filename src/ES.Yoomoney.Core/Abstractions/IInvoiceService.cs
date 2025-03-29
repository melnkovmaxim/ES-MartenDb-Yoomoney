using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Abstractions
{
    public interface IInvoiceService
    {
        Task<(Guid PaymentId, Uri ConfirmationUrl)> CreateInvoiceAsync(Guid accountId, decimal amount);
        Task<IReadOnlyCollection<Payment>> FetchPaymentsForCaptureAsync();
        Task CapturePaymentsAsync(params IEnumerable<string> paymentIds);
    }
}