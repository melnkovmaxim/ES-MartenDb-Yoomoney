using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using MediatR;

namespace ES.Yoomoney.Application.Features.IntegrationEvents;

internal sealed class InvoiceCreatedIntegrationEventHandler(IKafkaProducer<PaymentFailedIntegrationEvent> producer): INotificationHandler<OrderCreatedIntegrationEvent>
{
    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var authorizedPayment = new PaymentFailedIntegrationEvent(notification.OrderId, "Test");
        
        await producer.ProduceAsync(authorizedPayment, cancellationToken);
    }
}