using ES.Yoomoney.Core.DomainEvents;
using ES.Yoomoney.Core.OperationResult;
using ES.Yoomoney.Core.Projections;
using Marten;

namespace ES.Yoomoney.Infrastructure.Persistence;

public class EventStore(IDocumentSession session): IAsyncDisposable
{

    public async Task SaveEventsAsync<TEvent>(
        bool isExclusiveLock = false,
        params IReadOnlyCollection<TEvent> events)
        where TEvent: IDomainEvent
    {
        var streamId = ValidateAndGetStreamId(events);

        if (isExclusiveLock)
        {
            await session.Events.AppendExclusive(streamId, events);
        }
        else
        {
            session.Events.Append(streamId, events);
        }

        await session.SaveChangesAsync();
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