using Confluent.Kafka;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;
using MediatR;

namespace ES.Yoomoney.Infrastructure.Messaging.Consumers;

internal sealed class OrderCreatedEventsConsumer(IPublisher publisher): IMessageHandler<OrderCreatedIntegrationEvent>
{
    public const string Topic = nameof(OrderCreatedIntegrationEvent);
    
    public async Task Handle(IMessageContext context, OrderCreatedIntegrationEvent message)
    {
        await publisher.Publish(message);
    }
}