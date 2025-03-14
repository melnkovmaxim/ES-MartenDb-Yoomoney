using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.OperationResult;
using ES.Yoomoney.Core.Projections;
using Marten;
using IEventStore = Marten.Events.IEventStore;

namespace ES.Yoomoney.Infrastructure.Persistence.EventSourcing;

public sealed class EventStore(IDocumentSession session): IEsEventStore, IAsyncDisposable
{
    // public async Task<bool> ExistsAsync()
    // {
    //     await session.Events.
    // }
    
    public async Task AddEventAsync<TEvent>(
        TEvent @event,
        bool isExclusiveLock = false)
        where TEvent: IDomainEvent
    {
        if (isExclusiveLock)
        {
            await session.Events.AppendExclusive(@event.StreamId, @event);
        }
        else
        {
            session.Events.Append(@event.StreamId, @event);
        }
    }
    
    public async Task<Result<TProjection>> GetProjectionAndCreateSnapshotAsync<TProjection>(Guid streamId, CancellationToken ct)
        where TProjection : class, IApplicationProjection, new()
    {
        var snapshot = await session.LoadAsync<TProjection>(streamId, ct);
        var projection = snapshot is null
            ? await session.Events.AggregateStreamAsync<TProjection>(streamId, token: ct)
            : await session.Events.AggregateStreamAsync(streamId, state: snapshot, fromVersion: snapshot.Version + 1, token: ct);

        if (projection is null)
        {
            return Result<TProjection>.Fail("Projection missing");
        }

        if (projection.EventsCount >= 50)
        {
            session.Store(projection);

            await session.SaveChangesAsync(ct);
        }

        return Result<TProjection>.Success(projection);
    }

    private Guid ValidateAndGetStreamId<TEvent>(IReadOnlyCollection<TEvent> events)
        where TEvent : IDomainEvent
    {
        if (!events.Any())
        {
            throw new Exception("Cant save empty collection of domain events");
        }

        var streamId = events.First().StreamId;

        if (events.Any(e => e.StreamId != streamId))
        {
            throw new Exception("Cant save events to different streams in one transaction");
        }

        return streamId;
    }

    public async ValueTask DisposeAsync()
    {
        await session.DisposeAsync();
    }
}