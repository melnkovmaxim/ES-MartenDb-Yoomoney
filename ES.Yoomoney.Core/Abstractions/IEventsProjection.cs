namespace ES.Yoomoney.Core.Abstractions;

internal interface IEventsProjection<TProjection, TEvent>
    where TProjection: class
    where TEvent: class
{
    TProjection Apply(TEvent @event);
}

