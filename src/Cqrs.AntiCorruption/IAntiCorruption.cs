using System;
using System.Collections.Generic;
using Cqrs.Domain.Core;

namespace Cqrs.AntiCorruption
{
    public interface IAntiCorruption
    {
        IAntiCorruption Register<TSource, TDestination>(Func<TSource, TDestination> mapping);

        TEvent MapExternalEvent<TEvent>(object externalEvent) where TEvent : IEvent;

        IReadOnlyCollection<IEvent> MapExternalEvents(IReadOnlyCollection<object> externalEvents);

        object MapInternalEvent(IEvent internalEvent);

        IReadOnlyCollection<object> MapInternalEvents(IReadOnlyCollection<IEvent> internalEvents);

        Type GetMappedEventType(Type eventType);
    }
}
