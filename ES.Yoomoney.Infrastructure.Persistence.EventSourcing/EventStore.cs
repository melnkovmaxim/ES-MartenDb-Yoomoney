using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Core.OperationResult;
using ES.Yoomoney.Core.Projections;
using Marten;
using IEventStore = Marten.Events.IEventStore;

namespace ES.Yoomoney.Infrastructure.Persistence.EventSourcing;

public sealed class EventStore(IDocumentStore store): IEsEventStore
{
    public async Task StoreAsync(Aggregate aggregate, CancellationToken ct)
    {
        await using var session = await store.LightweightSerializableSessionAsync(token: ct);
        var events = aggregate.GetUncommittedEvents();
        session.Events.Append(aggregate.Id, aggregate.Version, events);
        await session.SaveChangesAsync(ct);
    }

    public async Task<T> LoadAsync<T>(
        Guid id,
        int? version,
        CancellationToken ct
    ) where T : Aggregate
    {
        await using var session = await store.LightweightSerializableSessionAsync(token: ct);
        var aggregate = await session.Events.AggregateStreamAsync<T>(id, version ?? 0, token: ct);
        return aggregate ?? throw new InvalidOperationException($"No aggregate by id {id}.");
    }
}