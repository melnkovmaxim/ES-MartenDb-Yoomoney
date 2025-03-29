using Confluent.Kafka;
using ES.Yoomoney.AdminPanel.Client.Pages;
using ES.Yoomoney.AdminPanel.Components;
using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Core.IntegrationEvents;
using ES.Yoomoney.Infrastructure.Clients.Extensions;
using ES.Yoomoney.Infrastructure.Messaging.Extensions;
using ES.Yoomoney.Infrastructure.Persistence.EventSourcing.Extensions;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// https://www.youtube.com/watch?v=O7oaxFgNuYo
// builder.AddKafkaProducer<string, string>("kafka");
var k = Environment.GetEnvironmentVariable("kafka-endpoint");
var config1 = new ProducerConfig()
{
    BootstrapServers = Environment.GetEnvironmentVariable("kafka-endpoint")
};

builder.Services.TryAddScoped(_ => new ConsumerBuilder<string, string>(new ConsumerConfig()
{
    BootstrapServers = Environment.GetEnvironmentVariable("kafka-endpoint"),
    GroupId = $"{Guid.CreateVersion7()}-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
}).Build());
builder.Services.AddSingleton(new ProducerBuilder<string, string>(config1).Build());
var config = new AdminClientConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("kafka-endpoint")
};

builder.Services.AddSingleton(new AdminClientBuilder(config).Build());
builder.Services
    .AddLayerApplication()
    .AddLayerInfrastructureClients()
    .AddLayerInfrastructurePersistenceEventSourcing()
    .AddInfrastructureMessagingLayer(builder.Configuration);

// Add services to the container.
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ES.Yoomoney.AdminPanel.Client._Imports).Assembly);

app.Run();