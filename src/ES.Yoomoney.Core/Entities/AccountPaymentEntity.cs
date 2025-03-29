using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.Entities;

public sealed record AccountPaymentEntity(string Id, Guid AccountId) : IEntity
{
    public bool IsProcessed { get; private set; }

    public void SetIsProcessed()
    {
        IsProcessed = true;
    }
}