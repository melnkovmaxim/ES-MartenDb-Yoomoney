using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Messaging.Consumers;
using ES.Yoomoney.Infrastructure.Messaging.Producers;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureMessagingLayer(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducer<PaymentAuthorizedIntegrationEvent>, PaymentAuthorizedEventsProducer>();
        services.AddSingleton<IKafkaProducer<PaymentFailedIntegrationEvent>, PaymentFailedEventsProducer>();
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(["kafka:9092"])
                        .CreateTopicIfNotExists(PaymentAuthorizedEventsProducer.Topic, 3, 1)
                        .CreateTopicIfNotExists(PaymentFailedEventsProducer.Topic, 3, 1)
                        .AddProducer<PaymentAuthorizedIntegrationEvent>(
                            producer => producer
                                .DefaultTopic(PaymentAuthorizedEventsProducer.Topic)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                        )
                        .AddProducer<PaymentFailedIntegrationEvent>(
                            producer => producer
                                .DefaultTopic(PaymentFailedEventsProducer.Topic)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                        )
                        .AddConsumer(
                            consumer => consumer
                                .Topic(OrderCreatedEventsConsumer.Topic)
                                .WithGroupId("es-yoomoney-group")
                                .WithBufferSize(1)
                                .WithWorkersCount(1)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                        .AddDeserializer<JsonCoreDeserializer>()
                                        .AddTypedHandlers(handlers => handlers
                                                .AddHandler<OrderCreatedEventsConsumer>()
                                        )
                                )
                        )
                )
        );

        return services;
    }
}