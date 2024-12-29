using ES.Yoomoney.Application.PaymentServices;
using ES.Yoomoney.Core.Projections;
using ES.Yoomoney.Infrastructure.Persistence;
using ES.Yoomoney.Infrastructure.Workers.Workers;
using ES.Yoomoney.ServiceDefaults;
using Marten;
using Yandex.Checkout.V3;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDataSource("postgresdb");
builder.Services.AddOpenApi();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<YoomoneyPaymentService>();
builder.Services.AddMarten(options =>
    {
        options.Events.AddEventType<Payment>();
    })
    .UseNpgsqlDataSource();
builder.Services.AddHostedService<FetchPaymentsWorker>();
builder.Services.AddSingleton<Client>(new Yandex.Checkout.V3.Client(
    shopId: "999274",
    secretKey: "test_PHF9bx_r4kRt7Zpf1U9ZaJ5CAhGqxcy0JwWjReNsBhs"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("payment", (Client client) =>
{
});

app.MapGet("payments/result", async (IDocumentStore store, EventStore<BalanceProjection> es) =>
{
    await using var session = store.LightweightSession();

    // var balance = await session.LoadAsync<BalanceProjection>(FetchPayments.UserId);
    // var balance = await session.Events.AggregateStreamAsync<BalanceProjection>(FetchPayments.UserId);
    
    
    return TypedResults.Ok(await es.GetAsync());
});

app.UseHttpsRedirection();

app.Run();