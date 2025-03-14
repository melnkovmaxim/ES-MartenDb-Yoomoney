using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Abstractions
{
    public interface IPaymentService
    {
        Task<(string PaymentId, string ConfirmationUrl)> CreateInvoiceAsync(decimal amount);
        Task<IReadOnlyCollection<Payment>> FetchPaymentsForCaptureAsync();
        Task CapturePaymentsAsync(params IEnumerable<string> paymentIds);
    }
}
