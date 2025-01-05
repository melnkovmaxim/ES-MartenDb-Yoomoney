using Confluent.Kafka;
using ES.Yoomoney.AdminPanel.Client.Pages;
using ES.Yoomoney.AdminPanel.Components;
using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Infrastructure.Clients.Extensions;
using ES.Yoomoney.Infrastructure.Messaging.Extensions;
using ES.Yoomoney.Infrastructure.Persistence.EventSourcing.Extensions;
using KafkaFlow.Serializer;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// https://www.youtube.com/watch?v=O7oaxFgNuYo
builder.AddKafkaProducer<Null, string>("kafka");

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