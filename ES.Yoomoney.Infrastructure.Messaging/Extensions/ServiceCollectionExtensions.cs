using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Core.Resources;
using ES.Yoomoney.Infrastructure.Messaging.Consumers;
using ES.Yoomoney.Infrastructure.Messaging.Middlewares;
using ES.Yoomoney.Infrastructure.Messaging.Producers;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureMessagingLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaFlowHostedService(kafka => kafka
            .UseConsoleLog()
            .AddCluster(
                cluster => cluster
                    .WithBrokers([configuration.GetConnectionString("kafka")])
                    .WithSecurityInformation(security => security.EnableSslCertificateVerification = false)
                    .CreateTopics()
                    .AddConsumers()
                    .AddProducers()
            ));

        return services;
    }

    private static IClusterConfigurationBuilder CreateTopics(this IClusterConfigurationBuilder builder)
    {
        return builder.CreateTopicIfNotExists(
            Resources.AppConstants.Topics.InvoiceStatusChanged, 1, 1);
    }

    private static IClusterConfigurationBuilder AddConsumers(this IClusterConfigurationBuilder builder)
    {
        return builder
            .AddConsumer(
                consumer => consumer
                    .Topic(Resources.AppConstants.Topics.InvoiceStatusChanged)
                    .WithGroupId(AppConstants.AppName)
                    .WithBufferSize(100)
                    .WithWorkersCount(3)
                    .AddMiddlewares(
                        middlewares => middlewares
                            .AddSingleTypeDeserializer<InvoiceStatusChangedIntegrationEvent, JsonCoreDeserializer>()
                            .Add<ErrorHandlingMiddleware>()
                            .AddTypedHandlers(handlers => handlers
                                .AddHandler<OrderCreatedEventsConsumer>()
                                .WhenNoHandlerFound(HandleException)
                            )
                    )
            );
    }

    private static IClusterConfigurationBuilder AddProducers(this IClusterConfigurationBuilder builder)
    {
        return builder.AddProducer<InvoiceStatusChangedIntegrationEvent>(
            producer => producer
                .DefaultTopic(Resources.AppConstants.Topics.InvoiceStatusChanged)
                .AddMiddlewares(m =>
                    m.AddSerializer<JsonCoreSerializer>()
                )
        );
    }

    private static void HandleException(IMessageContext context)
    {
        Console.WriteLine($"Message not handled {context.ConsumerContext.Partition} {context.ConsumerContext.Offset}");
        throw new Exception($"Message not handled {context.ConsumerContext.Partition} {context.ConsumerContext.Offset}");
    }
}