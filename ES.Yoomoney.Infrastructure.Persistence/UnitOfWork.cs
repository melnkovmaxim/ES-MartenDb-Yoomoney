using System.Collections.Concurrent;
using Marten;

namespace ES.Yoomoney.Infrastructure.Persistence
{
    public class UnitOfWork(IDocumentStore store)
    {
        private readonly ConcurrentDictionary<EventStore, IDocumentSession> _sessions = new();

        public EventStore CreateEventStore()
            => CreateEventStore(store.LightweightSession());

        public async Task<EventStore> CreateSerializableEventStoreAsync()
            => CreateEventStore(await store.LightweightSerializableSessionAsync());

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
}
