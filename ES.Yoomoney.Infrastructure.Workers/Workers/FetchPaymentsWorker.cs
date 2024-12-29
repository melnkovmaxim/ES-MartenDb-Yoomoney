using System.Data;
using ES.Yoomoney.Core.Projections;
using Marten;
using Microsoft.Extensions.Hosting;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Workers.Workers;

public sealed class FetchPaymentsWorker : BackgroundService
{
    private readonly Client _client;
    private readonly IDocumentStore _store;
    public static Guid UserId = Guid.NewGuid();

    public FetchPaymentsWorker(Client client, IDocumentStore store)
    {
        _client = client;
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
            var payments = _client.GetPayments(new PaymentFilter()
            {
                Status = PaymentStatus.WaitingForCapture
            }, new ListOptions()
            {
                PageSize = 100
            }).ToList();

            await using var session = _store.LightweightSession(IsolationLevel.RepeatableRead);

            var k = session.Events.Append(UserId, payments);

            await session.SaveChangesAsync(CancellationToken.None);

            var state = await session.Events.AggregateStreamAsync<BalanceProjection>(FetchPaymentsWorker.UserId);
            
            
            
            session.Store(state with { Version = k.Version });

            session.Events.Append(UserId, payments);

            await session.SaveChangesAsync();
    }
}