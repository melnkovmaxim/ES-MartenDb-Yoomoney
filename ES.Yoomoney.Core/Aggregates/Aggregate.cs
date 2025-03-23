namespace ES.Yoomoney.Core.Aggregates;

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; private set; }
    protected List<Events.Event> UncommittedEvents { get; } = [];
    protected List<Events.Event> CommittedEvents { get; } = [];

    public IReadOnlyCollection<Events.Event> GetUncommittedEvents()
    {
        var uncommittedEvents = UncommittedEvents.ToArray();
        
        UncommittedEvents.Clear();
        
        return uncommittedEvents;
    }

    public IReadOnlyCollection<Events.Event> GetCommittedEvents()
    {
        return CommittedEvents;
    }

    protected void IncreaseVersion()
    {
        Version++;
    }
}