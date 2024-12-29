using ES.Yoomoney.Core.Abstractions;
using Marten;

namespace ES.Yoomoney.Infrastructure.Persistence;

public sealed class GenericRepository<TEntity>(IDocumentStore store): IRepository<TEntity> where TEntity: IEntity
{
    public Task<TEntity?> GetFirstOrDefaultAsync(string id, CancellationToken ct)
    {
        var session = store.LightweightSession();

        return session.Query<TEntity>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpsertAsync(TEntity entity, CancellationToken ct)
    {
        var session = store.LightweightSession();
        
        session.Store(entity);

        await session.SaveChangesAsync(ct);
    }
}