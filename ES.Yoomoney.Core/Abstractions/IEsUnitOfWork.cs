namespace ES.Yoomoney.Core.Abstractions;

public interface IEsUnitOfWork
{
    IEsEventStore CreateEventStore();
    Task<IEsEventStore> CreateSerializableEventStoreAsync();
    Task CommitAsync();
}