using System;
using System.Collections.Generic;
using Cqrs.Domain.Core;

namespace Cqrs.Messaging.Core
{
    public interface IEventBus
    {
        IDisposable Subscribe<TEvent>() where TEvent : IEvent;

        void Publish(IReadOnlyCollection<IEvent> events);
        
        void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent;
    }
}
