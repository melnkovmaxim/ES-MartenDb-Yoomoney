namespace ES.Yoomoney.Core.DomainEvents
{
    public interface IDomainEvent
    {
        Guid StreamId { get; }
    }
}
