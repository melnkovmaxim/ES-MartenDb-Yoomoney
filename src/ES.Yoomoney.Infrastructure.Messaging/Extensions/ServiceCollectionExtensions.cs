using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Messaging.Consumers;
using ES.Yoomoney.Infrastructure.Messaging.Middlewares;
using ES.Yoomoney.Infrastructure.Messaging.Resources;

using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureMessagingLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = services.AddKafkaFlowHostedService(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers([configuration.GetConnectionString("kafka")])
                        .WithSecurityInformation(security => security.EnableSslCertificateVerification = false)
                        .CreateTopics()
                        .AddConsumers()
                        .AddProducers()));

        return services;
    }

    private static IClusterConfigurationBuilder CreateTopics(this IClusterConfigurationBuilder builder) =>
        builder.CreateTopicIfNotExists(
            AppConstants.Topics.InvoiceStatusChanged,
            numberOfPartitions: 1,
            replicationFactor: 1);

    private static IClusterConfigurationBuilder AddConsumers(this IClusterConfigurationBuilder builder)
    {
        return builder
            .AddConsumer(
                consumer => consumer
                    .Topic(AppConstants.Topics.InvoiceStatusChanged)
                    .WithGroupId(Core.Resources.AppConstants.AppName)
                    .WithBufferSize(100)
                    .WithWorkersCount(3)
                    .AddMiddlewares(
                        middlewares => middlewares
                            .AddSingleTypeDeserializer<InvoiceStatusChangedIntegrationEvent, JsonCoreDeserializer>()
                            .Add<ErrorHandlingMiddleware>()
                            .AddTypedHandlers(
                                handlers => handlers
                                    .AddHandler<InvoiceStatusChangedConsumer>()
                                    .WhenNoHandlerFound(HandleException))));
    }

    private static IClusterConfigurationBuilder AddProducers(this IClusterConfigurationBuilder builder)
    {
        return builder.AddProducer<InvoiceStatusChangedIntegrationEvent>(
            producer => producer
                .DefaultTopic(AppConstants.Topics.InvoiceStatusChanged)
                .AddMiddlewares(
                    m =>
                        m.AddSerializer<JsonCoreSerializer>()));
    }

    private static void HandleException(IMessageContext context)
    {
        Console.WriteLine($"Message not handled {context.ConsumerContext.Partition} {context.ConsumerContext.Offset}");
#pragma warning disable S112
#pragma warning disable CA2201
        throw new Exception(
            $"Message not handled {context.ConsumerContext.Partition} {context.ConsumerContext.Offset}");
#pragma warning restore CA2201
#pragma warning restore S112
    }
}