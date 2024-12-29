using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ES.Yoomoney.Infrastructure.Workers.Workers;

public sealed class PaymentsPaidProcessingWorker(IServiceProvider sp): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = sp.CreateScope();

        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        
        while (!ct.IsCancellationRequested)
        {
            var capturedPayments = await paymentService.FetchPaymentsForCaptureAsync();
            var paymentPaidEvent = new PaymentPaidIntegrationEvent(capturedPayments);

            await publisher.Publish(paymentPaidEvent, ct);
            
            await Task.Delay(2000, ct);
        }
    }
}