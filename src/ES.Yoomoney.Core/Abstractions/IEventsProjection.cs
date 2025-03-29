namespace ES.Yoomoney.Core.Abstractions;

internal interface IEventsProjection<out TProjection, in TEvent>
    where TProjection : class
    where TEvent : class
{
    TProjection Apply(TEvent @event);
}