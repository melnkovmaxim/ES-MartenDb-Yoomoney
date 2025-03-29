using ES.Yoomoney.Common.Core.Exceptions;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;

using Marten;

namespace ES.Yoomoney.Infrastructure.Persistence.EventSourcing;

public sealed class EventStore(IDocumentStore store) : IEsEventStore
{
    public async Task<bool> ExistsAsync(Guid eventId, CancellationToken ct)
    {
        await using var session = store.LightweightSession();

        var existingEvent = await session.Query<DomainEvents.Event>()
            .Where(e => e.StreamId == eventId)
            .FirstOrDefaultAsync(ct);

        return existingEvent is not null;
    }

    public async Task StoreAsync(Aggregate aggregate, CancellationToken ct)
    {
        await using var session = await store.LightweightSerializableSessionAsync(token: ct);

        var events = aggregate.GetUncommittedEvents();

        // TODO: выпилить когда добавим unique index / key
        if (events.DistinctBy(x => x.EventId).Count() != events.Count)
        {
            throw new DomainException("Has duplicated");
        }

        _ = session.Events.Append(aggregate.Id, aggregate.Version, events);

        await session.SaveChangesAsync(ct);
    }

    public async Task<T?> LoadAsync<T>(
        Guid id,
        int? version,
        CancellationToken ct) where T : Aggregate
    {
        await using var session = await store.LightweightSerializableSessionAsync(token: ct);

        var aggregate = await session.Events.AggregateStreamAsync<T>(id, version ?? 0, token: ct);

        return aggregate;
    }
}