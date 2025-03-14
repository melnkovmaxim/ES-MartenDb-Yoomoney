using ES.Yoomoney.Core.Aggregates;

namespace ES.Yoomoney.Core.Abstractions;

internal interface IApplyEvent<out TAggregate, in TEvent>
    where TEvent: Events.Event
{
    TAggregate Apply(TEvent @event);
}