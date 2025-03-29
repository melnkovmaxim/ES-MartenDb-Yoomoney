using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Workers.Options;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Workers.Workers;

public sealed class PaymentsPaidProcessingWorker(IServiceProvider sp) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = sp.CreateScope();

        var options = scope.ServiceProvider.GetRequiredService<IOptions<BackgroundWorkerOptions>>();
        var paymentService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        while (!ct.IsCancellationRequested)
        {
            var invoices = await paymentService.FetchPaymentsForCaptureAsync();

            await PublishEvents(publisher, invoices, ct);

            await Task.Delay(options.Value.FetchPaymentPeriod, ct);
        }
    }

    private static async Task PublishEvents(
        IPublisher publisher,
        IReadOnlyCollection<Payment> invoices,
        CancellationToken ct)
    {
        foreach (var invoice in invoices)
        {
            var paymentPaidEvent = new InvoiceStatusChangedIntegrationEvent(invoice);

            await publisher.Publish(paymentPaidEvent, ct);
        }
    }
}