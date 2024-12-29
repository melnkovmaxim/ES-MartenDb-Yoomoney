
using ES.Yoomoney.Core.Abstractions;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Application.PaymentServices
{
    public class YoomoneyPaymentService(Client client) : IPaymentService
    {
        public Task<(string PaymentId, string ConfirmationUrl)> CreatePaymentAsync(decimal amount)
        {
            var newPayment = new NewPayment()
            {
                Confirmation = new Confirmation()
                {
                    ReturnUrl = "http://localhost",
                    Enforce = true
                },
                PaymentMethodData = new PaymentMethod()
                {
                    Type = PaymentMethodType.BankCard
                },
                Amount = new Amount()
                {
                    Currency = "RUB",
                    Value = amount
                },
                Capture = false
            };

            var createdPayment = client.CreatePayment(newPayment);
            var result = (PaymentId: createdPayment.Id, createdPayment.Confirmation.ConfirmationUrl);

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<Payment>> FetchUnprocessedPaymentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task MarkPaymentsAsProcessedAsync(IEnumerable<string> paymentIds)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Payment>> FetchPaymentsForCaptureAsync()
        {
            var filter = new PaymentFilter()
            {
                Status = PaymentStatus.WaitingForCapture
            };
            var options = new ListOptions()
            {
                PageSize = 50
            };

            var payments = client.GetPayments(filter, options).ToArray();

            return Task.FromResult(payments as IReadOnlyCollection<Payment>);
        }

        public Task CapturePaymentsAsync(IEnumerable<string> paymentIds)
        {
            foreach (var paymentId in paymentIds) 
            {
                client.CapturePayment(paymentId);
            }

            return Task.CompletedTask;
        }
    }
}
