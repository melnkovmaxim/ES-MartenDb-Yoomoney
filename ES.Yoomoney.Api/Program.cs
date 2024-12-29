using ES.Yoomoney.Api;
using ES.Yoomoney.Api.Extensions;
using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Infrastructure.Clients.Extensions;
using ES.Yoomoney.Infrastructure.Persistence.Extensions;
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
    .AddLayerInfrastructurePersistence()
    .AddLayerInfrastructureWorkers()
    .AddLayerInfrastructureClients();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapApplicationEndpoints();

app.Run();