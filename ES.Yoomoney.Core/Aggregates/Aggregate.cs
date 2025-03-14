namespace ES.Yoomoney.Core.Aggregates;

public abstract class Aggregate
{
    public Guid StreamId { get; private set; }
    public int Version { get; private set; }
    protected List<Events.Event> Events { get; } = [];

    public IReadOnlyCollection<Events.Event> GetUncommittedEvents()
    {
        var uncommitedEvents = Events.ToArray();
        
        Events.Clear();
        
        return uncommitedEvents;
    }
}