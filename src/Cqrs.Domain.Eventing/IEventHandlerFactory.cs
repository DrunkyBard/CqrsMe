using System.Collections.Generic;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.Eventing
{
    public interface IEventHandlerFactory
    {
        IReadOnlyCollection<IDomainEventHandler<TEvent>> Create<TEvent>() where TEvent : IEvent;
    }
}
