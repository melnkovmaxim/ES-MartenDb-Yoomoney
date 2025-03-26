using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Workers.Workers;

public sealed class PaymentsPaidProcessingWorker(IServiceProvider sp): BackgroundService
{
    private static readonly TimeSpan _delayTime = TimeSpan.FromHours(1); 
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = sp.CreateScope();

        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        
        while (!ct.IsCancellationRequested)
        {
            var invoices = await paymentService.FetchPaymentsForCaptureAsync();

            await PublishEvents(publisher, invoices, ct);
            
            await Task.Delay(_delayTime, ct);
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