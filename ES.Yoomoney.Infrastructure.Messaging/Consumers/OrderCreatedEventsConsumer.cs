using System.Text.Json;
using Confluent.Kafka;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;
using MediatR;

namespace ES.Yoomoney.Infrastructure.Messaging.Consumers;

public sealed class OrderCreatedEventsConsumer: IMessageHandler<OrderCreatedIntegrationEvent>
{
    public const string Topic = nameof(OrderCreatedIntegrationEvent);

    public OrderCreatedEventsConsumer()
    {
        
    }
    
    public async Task Handle(IMessageContext context, OrderCreatedIntegrationEvent message)
    {
        Console.WriteLine(JsonSerializer.Serialize(message));
        // await publisher.Publish(message);
    }
}