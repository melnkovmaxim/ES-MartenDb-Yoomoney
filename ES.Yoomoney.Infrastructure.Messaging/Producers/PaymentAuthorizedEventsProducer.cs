using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;

namespace ES.Yoomoney.Infrastructure.Messaging.Producers;

internal sealed class PaymentAuthorizedEventsProducer(IMessageProducer<PaymentAuthorizedIntegrationEvent> producer): 
    IKafkaProducer<PaymentAuthorizedIntegrationEvent>
{
    public const string Topic = nameof(PaymentAuthorizedIntegrationEvent);
    
    public async Task ProduceAsync(PaymentAuthorizedIntegrationEvent @event, CancellationToken ct)
    {
        await producer.ProduceAsync(messageKey: @event.OrderId.ToString(), messageValue: @event);
    }
}