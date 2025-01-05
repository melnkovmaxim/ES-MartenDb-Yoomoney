using System.Text.Json;
using Confluent.Kafka;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;
using MediatR;

namespace ES.Yoomoney.Infrastructure.Messaging.Consumers;

public sealed class OrderCreatedEventsConsumer: IMessageHandler<string>
{
    public const string Topic = nameof(OrderCreatedIntegrationEvent);
    
    public async Task Handle(IMessageContext context, string message)
    {
        Console.WriteLine(JsonSerializer.Serialize(message));
        // await publisher.Publish(message);
    }
}