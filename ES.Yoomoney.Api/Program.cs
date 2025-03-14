using ES.Yoomoney.Api;
using ES.Yoomoney.Api.Extensions;
using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Infrastructure.Clients.Extensions;
using ES.Yoomoney.Infrastructure.Messaging.Extensions;
using ES.Yoomoney.Infrastructure.Persistence.EventSourcing.Extensions;
using ES.Yoomoney.Infrastructure.Workers.Extensions;
using ES.Yoomoney.AdminPanel.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDataSource("postgresdb");
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpoints(CurrentAssembly.Reference);

builder.Services
    .AddLayerApplication()
    .AddLayerInfrastructurePersistenceEventSourcing()
    .AddLayerInfrastructureWorkers()
    .AddLayerInfrastructureClients()
    .AddInfrastructureMessagingLayer(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapApplicationEndpoints();

app.Run();

#pragma warning disable CA1050 // Declare types in namespaces
public partial class Program
{
}
#pragma warning restore CA1050 // Declare types in namespaces