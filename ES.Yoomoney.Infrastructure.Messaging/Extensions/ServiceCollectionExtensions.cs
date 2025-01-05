using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Messaging.Consumers;
using ES.Yoomoney.Infrastructure.Messaging.Middlewares;
using ES.Yoomoney.Infrastructure.Messaging.Producers;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ES.Yoomoney.Infrastructure.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureMessagingLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IKafkaProducer<PaymentAuthorizedIntegrationEvent>, PaymentAuthorizedEventsProducer>();
        services.AddSingleton<IKafkaProducer<PaymentFailedIntegrationEvent>, PaymentFailedEventsProducer>();
        services.AddKafkaFlowHostedService(kafka => kafka
            .UseConsoleLog()
            .AddCluster(
                cluster => cluster
                    .WithBrokers([configuration.GetConnectionString("kafka")])
                    .WithSecurityInformation(security => security.EnableSslCertificateVerification = false)
                    .CreateTopicIfNotExists(PaymentAuthorizedEventsProducer.Topic, 1, 1)
                    .CreateTopicIfNotExists(PaymentFailedEventsProducer.Topic, 1, 1)
                    .CreateTopicIfNotExists(OrderCreatedEventsConsumer.Topic, 1, 1)
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
                            .WithBufferSize(100)
                            .WithWorkersCount(3)
                            .AddMiddlewares(
                                middlewares => middlewares
                                    .AddTypedHandlers(handlers => handlers
                                        .AddHandler<OrderCreatedEventsConsumer>()
                                        .WhenNoHandlerFound(context =>
                                            Console.WriteLine($"Message not handled {context.ConsumerContext.Partition} {context.ConsumerContext.Offset}"))
                                    )
                            )
                    )
            ));

        return services;
    }
}