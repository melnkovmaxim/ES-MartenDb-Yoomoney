using ES.Yoomoney.Core.Abstractions;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Clients;

public class YoomoneyInvoiceService(Client client) : IInvoiceService
{
    public Task<IReadOnlyCollection<Payment>> FetchPaymentsForCaptureAsync()
    {
        var filter = new PaymentFilter
        {
            Status = PaymentStatus.WaitingForCapture
        };
        var options = new ListOptions
        {
            PageSize = 50
        };

        var payments = client.GetPayments(filter, options).ToArray();

        return Task.FromResult(payments as IReadOnlyCollection<Payment>);
    }

    public Task CapturePaymentsAsync(params IEnumerable<string> paymentIds)
    {
        foreach (var paymentId in paymentIds)
        {
            _ = client.CapturePayment(paymentId);
        }

        return Task.CompletedTask;
    }

    public Task<(Guid PaymentId, Uri ConfirmationUrl)> CreateInvoiceAsync(
        Guid accountId,
        decimal amount)
    {
        var newPayment = new NewPayment
        {
            Confirmation = new Confirmation
            {
                ReturnUrl = "http://localhost",
                Enforce = true
            },
            Amount = new Amount
            {
                Currency = "RUB",
                Value = amount
            },
            MerchantCustomerId = accountId.ToString(),
            Capture = false
        };

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            newPayment.PaymentMethodData = new PaymentMethod
            {
                Type = PaymentMethodType.BankCard,
                Card = new Card
                {
                    Number = "5555555555554444",
                    ExpiryMonth = "01",
                    ExpiryYear = "2029",
                    Csc = "234"
                }
            };
        }

        var createdPayment = client.CreatePayment(newPayment);
        var result = (PaymentId: Guid.Parse(createdPayment.Id), new Uri(createdPayment.Confirmation.ConfirmationUrl));

        return Task.FromResult(result);
    }
}