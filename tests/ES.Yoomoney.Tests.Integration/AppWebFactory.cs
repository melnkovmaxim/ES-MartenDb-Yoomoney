using System.Collections.Concurrent;
using Confluent.Kafka;
using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Messaging.Consumers;
using KafkaFlow;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;

namespace ES.Yoomoney.Tests.Integration;

public sealed class AppWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithPassword("Strong_password_123!")
        .Build();

    private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
        .WithImage("confluentinc/cp-kafka:7.8.0")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _kafkaContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _kafkaContainer.StopAsync();
    }

    public T Resolve<T>() where T : notnull =>
        Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope()
            .ServiceProvider.GetRequiredService<T>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:postgresdb", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:kafka", _kafkaContainer.GetBootstrapAddress());

        builder.ConfigureServices(
            services =>
            {
                services.AddSingleton<IProducer<string, string>>(
                    sp =>
                    {
                        var producerConfig = new ProducerConfig
                        {
                            BootstrapServers = _kafkaContainer.GetBootstrapAddress()
                        };

                        return new ProducerBuilder<string, string>(producerConfig)
                            .Build();
                    });
                services.AddSingleton<ConcurrentQueue<InvoiceStatusChangedIntegrationEvent>>();
                services.RemoveAll<InvoiceStatusChangedConsumer>();
                services.AddScoped<InvoiceStatusChangedConsumer>(
                    sp =>
                    {
                        var mock = new Mock<InvoiceStatusChangedConsumer>();

                        mock.Setup(
                                m => m.Handle(
                                    It.IsAny<IMessageContext>(),
                                    It.IsAny<InvoiceStatusChangedIntegrationEvent>()))
                            .Callback<IMessageContext, InvoiceStatusChangedIntegrationEvent>(
                                (_, message) =>
                                {
                                    var queue = sp
                                        .GetRequiredService<ConcurrentQueue<InvoiceStatusChangedIntegrationEvent>>();

                                    queue.Enqueue(message);
                                })
                            .Returns(Task.CompletedTask);

                        return mock.Object;
                    });
            });
    }
}