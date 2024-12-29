using ES.Yoomoney.Core.OperationResult;

namespace ES.Yoomoney.Core.Abstractions;

public interface IRepository<TEntity> where TEntity: IEntity
{
    Task<TEntity?> GetFirstOrDefaultAsync(string id, CancellationToken ct);
    Task UpsertAsync(TEntity entity, CancellationToken ct);
}