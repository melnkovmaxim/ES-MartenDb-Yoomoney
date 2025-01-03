using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;

namespace ES.Yoomoney.Infrastructure.Messaging.Producers;

internal sealed class PaymentFailedEventsProducer(IMessageProducer<PaymentFailedIntegrationEvent> producer): 
    IKafkaProducer<PaymentFailedIntegrationEvent>
{
    public const string Topic = nameof(PaymentFailedIntegrationEvent);
    
    public async Task ProduceAsync(PaymentFailedIntegrationEvent @event, CancellationToken ct)
    {
        await producer.ProduceAsync(messageKey: @event.OrderId.ToString(), messageValue: @event);
    }
}