using MediatR;

namespace ES.Yoomoney.Core.Abstractions
{
    public interface IDomainEvent : INotification
    {
        Guid StreamId { get; }
    }
}