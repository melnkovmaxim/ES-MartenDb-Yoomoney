namespace ES.Yoomoney.Core.Aggregates;

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; private set; }
    protected HashSet<DomainEvents.Event> UncommittedEvents { get; } = [];
    protected HashSet<DomainEvents.Event> TotalEvents { get; } = [];

    public IReadOnlyCollection<DomainEvents.Event> GetUncommittedEvents()
    {
        var uncommittedEvents = UncommittedEvents.ToArray();

        return uncommittedEvents;
    }

    public IReadOnlyCollection<DomainEvents.Event> GetCommittedEvents() => TotalEvents;

    protected void IncreaseVersion()
    {
        Version++;
    }
}