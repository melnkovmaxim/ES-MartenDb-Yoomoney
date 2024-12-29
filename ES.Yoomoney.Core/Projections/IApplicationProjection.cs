namespace ES.Yoomoney.Core.Projections;

public interface IApplicationProjection
{
    Guid AccountId { get; init; }
    long Version { get; }
    int EventsCount { get; }
}