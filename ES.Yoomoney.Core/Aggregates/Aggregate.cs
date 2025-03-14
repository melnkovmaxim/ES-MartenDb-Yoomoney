namespace ES.Yoomoney.Core.Aggregates;

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; private set; }
    protected List<Events.Event> Events { get; } = [];

    public IReadOnlyCollection<Events.Event> GetUncommittedEvents()
    {
        var uncommitedEvents = Events.ToArray();
        
        Events.Clear();
        
        return uncommitedEvents;
    }

    protected void IncreaseVersion()
    {
        Version++;
    }
}