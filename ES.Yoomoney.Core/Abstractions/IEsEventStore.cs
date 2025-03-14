using ES.Yoomoney.Core.OperationResult;
using ES.Yoomoney.Core.Projections;

namespace ES.Yoomoney.Core.Abstractions;

public interface IEsEventStore: IAsyncDisposable
{
    Task AddEventAsync<TEvent>(
        TEvent @event,
        bool isExclusiveLock = false)
        where TEvent: IDomainEvent;
    
    Task<Result<TProjection>> GetProjectionAndCreateSnapshotAsync<TProjection>(Guid streamId, CancellationToken ct)
        where TProjection : class, IApplicationProjection, new();
}