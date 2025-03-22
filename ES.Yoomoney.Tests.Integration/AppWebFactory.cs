using System.Collections.Concurrent;
using System.Reflection;
using Aspire.Npgsql;
using Confluent.Kafka;
using ES.Yoomoney.Api;
using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Messaging.Consumers;
using ES.Yoomoney.Tests.Integration.ConsumerTests;
using Google.Protobuf.WellKnownTypes;
using KafkaFlow;
using KafkaFlow.Serializer;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Tests.Integration;

public sealed class AppWebFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    public T Resolve<T>() where T : notnull => Services.GetRequiredService<IServiceScopeFactory>()
        .CreateScope().ServiceProvider.GetRequiredService<T>();
    
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithPassword("Strong_password_123!")
        .Build();

    private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
        .WithImage("confluentinc/cp-kafka:7.8.0")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:postgresdb", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:kafka", _kafkaContainer.GetBootstrapAddress());


        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IProducer<string, string>>(sp =>
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = _kafkaContainer.GetBootstrapAddress(),
                };

                return new ProducerBuilder<string, string>(producerConfig)
                    .Build();
            });
            services.AddSingleton<ConcurrentQueue<OrderCreatedIntegrationEvent>>();
            services.RemoveAll(typeof(OrderCreatedEventsConsumer));
            services.AddScoped<OrderCreatedEventsConsumer>(sp =>
            {
                var mock = new Mock<OrderCreatedEventsConsumer>();

                mock.Setup(m => m.Handle(It.IsAny<IMessageContext>(), It.IsAny<OrderCreatedIntegrationEvent>()))
                    .Callback<IMessageContext, OrderCreatedIntegrationEvent>((_, messageeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee) =>
                    {
                        var queue = sp.GetRequiredService<ConcurrentQueue<OrderCreatedIntegrationEvent>>();
                        
                        queue.Enqueue(message);
                    })
                    .Returns(Task.CompletedTask); 

                return mock.Object;
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _kafkaContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _kafkaContainer.StopAsync();
    }
}