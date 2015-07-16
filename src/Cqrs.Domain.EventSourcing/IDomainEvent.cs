using Cqrs.Domain.Core;

namespace Cqrs.Domain.EventSourcing
{
    public interface IDomainEvent : IEvent, IEventSourced
    {
    }
}
