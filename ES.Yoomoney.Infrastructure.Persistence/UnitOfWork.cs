using System.Collections.Concurrent;
using ES.Yoomoney.Core.Abstractions;
using Marten;
using Marten.Services;

namespace ES.Yoomoney.Infrastructure.Persistence;

public sealed class UnitOfWork(IDocumentStore store): IEsUnitOfWork
{
    private readonly ConcurrentDictionary<EventStore, IDocumentSession> _sessions = new();

    public IEsEventStore CreateEventStore()
        => CreateEventStore(store.LightweightSession());

    public async Task<IEsEventStore> CreateSerializableEventStoreAsync()
        => CreateEventStore(await store.LightweightSerializableSessionAsync());

    public async Task CommitAsync()
    {
        foreach (var pair in _sessions)
        {
            await pair.Value.SaveChangesAsync(CancellationToken.None);
        }
    }

    private EventStore CreateEventStore(IDocumentSession session)
    {
        var eventStore = new EventStore(session);

        if (!_sessions.TryAdd(eventStore, session))
        {
            throw new Exception("Event store key already exists in session dictionary");
        }

        return eventStore;
    }
}