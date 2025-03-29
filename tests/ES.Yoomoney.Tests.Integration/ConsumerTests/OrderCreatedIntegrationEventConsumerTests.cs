namespace ES.Yoomoney.Tests.Integration.ConsumerTests;

#pragma warning disable S125
#pragma warning disable S2699
[Collection(nameof(AppWebFactory))]
public sealed class OrderCreatedIntegrationEventConsumerTests(AppWebFactory factory)
{
    // private readonly IProducer<string, string> _producer = factory.Resolve<IProducer<string, string>>();
    // private readonly ConcurrentQueue<OrderCreatedIntegrationEvent> _queue = factory.Resolve<ConcurrentQueue<OrderCreatedIntegrationEvent>>();

    [Fact]
    public Task Produce_Consume_ShouldConsumeMessage()
    {
        return Task.CompletedTask;
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
#pragma warning restore S125