using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Domain.Core;
using Cqrs.ObjectMapping;

namespace Cqrs.AntiCorruption
{
    internal sealed class AntiCorruption : IAntiCorruption
    {
        private readonly IObjectMapper _objectMapper;

        private static Dictionary<Type, Func<object>> _mappings; 

        public AntiCorruption(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public IAntiCorruption Register<TSource, TDestination>(Func<TSource, TDestination> mapping)
        {
            throw new NotImplementedException();
        }

        public TEvent MapExternalEvent<TEvent>(object externalEvent) where TEvent : IEvent
        {
            var internalEvent = (TEvent)_objectMapper.Map(externalEvent);

            return internalEvent;
        }

        public IReadOnlyCollection<IEvent> MapExternalEvents(IReadOnlyCollection<object> externalEvents)
        {
            var internalEvents = externalEvents
                .Select(externalEvent => (IEvent)_objectMapper.Map(externalEvent))
                .ToList();

            return internalEvents;
        }

        public object MapInternalEvent(IEvent internalEvent)
        {
            return _objectMapper.Map(internalEvent);
        }

        public IReadOnlyCollection<object> MapInternalEvents(IReadOnlyCollection<IEvent> internalEvents)
        {
            var externalEvents = internalEvents
                .Select(internalEvent => _objectMapper.Map(internalEvent))
                .ToList();

            return externalEvents;
        }

        public Type GetMappedEventType(Type eventType)
        {
            return _objectMapper.GetMappedType(eventType);
        }
    }
}
