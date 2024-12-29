using ES.Yoomoney.Core.Abstractions;
using MediatR;

namespace ES.Yoomoney.Core.DomainEvents
{
    public sealed record TopupBalanceDomainEvent(Guid StreamId, decimal Amount): IDomainEvent, INotification;
}
