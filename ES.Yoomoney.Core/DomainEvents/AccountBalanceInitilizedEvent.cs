using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.DomainEvents;

public sealed record AccountBalanceInitializedEvent(Guid StreamId): IDomainEvent;