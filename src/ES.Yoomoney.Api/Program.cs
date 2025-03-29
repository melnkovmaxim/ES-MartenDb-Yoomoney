using ES.Yoomoney.Api;
using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Common.Presentation.MinimapApi.Extensions;
using ES.Yoomoney.Infrastructure.Clients.Extensions;
using ES.Yoomoney.Infrastructure.Messaging.Extensions;
using ES.Yoomoney.Infrastructure.Persistence.EventSourcing.Extensions;
using ES.Yoomoney.Infrastructure.Workers.Extensions;
using ES.Yoomoney.ServiceDefaults;

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
    _ = app.MapOpenApi();
    _ = app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapApplicationEndpoints();

#pragma warning disable S6966
app.Run();
#pragma warning restore S6966

internal abstract partial class Program
{
}