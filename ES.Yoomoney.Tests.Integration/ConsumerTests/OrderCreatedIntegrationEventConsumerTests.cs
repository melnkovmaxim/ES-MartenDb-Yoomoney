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
    // private readonly IProducer<string, string> _producer = factory.Resolve<IProducer<string, string>>();
    // private readonly ConcurrentQueue<OrderCreatedIntegrationEvent> _queue = factory.Resolve<ConcurrentQueue<OrderCreatedIntegrationEvent>>();
    
    [Fact]
    public async Task Produce_Consume_ShouldConsumeMessage()
    {
        // var producer = factory.Resolve<IMessageProducer<OrderCreatedIntegrationEvent>>();
        // var result1 = await producer.ProduceAsync(nameof(OrderCreatedIntegrationEvent), new Message<string, string>()
        // {
        //     Key = nameof(OrderCreatedIntegrationEvent),
        //     Value = JsonSerializer.Serialize(new OrderCreatedIntegrationEvent(Guid.CreateVersion7(), 100))
        // });
        // var result = await _producer.ProduceAsync(nameof(OrderCreatedIntegrationEvent), new Message<string, string>()
        // {
        //     Key = nameof(OrderCreatedIntegrationEvent),
        //     Value = JsonSerializer.Serialize(new OrderCreatedIntegrationEvent(Guid.CreateVersion7(), 100))
        // });
        //
        // // var handler = factory.Resolve<IMessageHandler<OrderCreatedIntegrationEvent>>();
        // // await handler.Handle(null, new OrderCreatedIntegrationEvent(Guid.CreateVersion7(), 100));
        //
        // await Task.Delay(1000);
        //
        // _queue.Should().HaveCount(1);
    }
}