using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Core.OperationResult;
using ES.Yoomoney.Core.Projections;

namespace ES.Yoomoney.Core.Abstractions;

public interface IEsEventStore
{
    Task<bool> ExistsAsync(Guid eventId, CancellationToken ct);
    
    Task StoreAsync(Aggregate aggregate, CancellationToken ct);

    Task<T?> LoadAsync<T>(
        Guid id,
        int? version,
        CancellationToken ct
    ) where T : Aggregate;
}