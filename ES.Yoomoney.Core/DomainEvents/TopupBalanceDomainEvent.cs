namespace ES.Yoomoney.Core.DomainEvents
{
    public record TopupBalanceDomainEvent(Guid StreamId, decimal Amount): IDomainEvent;
}
