using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using MediatR;

namespace ES.Yoomoney.Application.Features.IntegrationEvents;

internal sealed class OrderCreatedIntegrationEventHandler(IKafkaProducer<PaymentAuthorizedIntegrationEvent> producer): INotificationHandler<OrderCreatedIntegrationEvent>
{
    public async Task Handle(OrderCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var authorizedPayment = new PaymentAuthorizedIntegrationEvent(notification.OrderId);
        
        await producer.ProduceAsync(authorizedPayment, cancellationToken);
    }
}