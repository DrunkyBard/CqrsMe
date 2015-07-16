using Cqrs.Domain.Core;

namespace Cqrs.Domain.Eventing
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
