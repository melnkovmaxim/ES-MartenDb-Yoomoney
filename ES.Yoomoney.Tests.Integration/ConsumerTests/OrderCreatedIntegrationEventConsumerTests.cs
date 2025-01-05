using System.Collections.Concurrent;
using System.Text.Json;
using Confluent.Kafka;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using FluentAssertions;
using KafkaFlow;

namespace ES.Yoomoney.Tests.Integration.ConsumerTests;

[Collection(nameof(AppWebFactory))]
public sealed class OrderCreatedIntegrationEventConsumerTests(AppWebFactory factory)
{
    private readonly IProducer<Null, string> _producer = factory.Resolve<IProducer<Null, string>>();
    private readonly ConcurrentQueue<OrderCreatedIntegrationEvent> _queue = factory.Resolve<ConcurrentQueue<OrderCreatedIntegrationEvent>>();
    
    [Fact]
    public async Task Produce_Consume_ShouldConsumeMessage()
    {
        _producer.Produce(nameof(OrderCreatedIntegrationEvent), new Message<Null, OrderCreatedIntegrationEvent>()
        {
            Value = JsonSerializer.Serialize(new OrderCreatedIntegrationEvent(Guid.NewGuid(), 100))
        });

        // var handler = factory.Resolve<IMessageHandler<OrderCreatedIntegrationEvent>>();
        // await handler.Handle(null, new OrderCreatedIntegrationEvent(Guid.NewGuid(), 100));

        await Task.Delay(1000);

        _queue.Should().HaveCount(1);
    }
}