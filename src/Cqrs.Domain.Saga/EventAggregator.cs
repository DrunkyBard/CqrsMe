using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.ProcessManager
{
    public abstract class EventAggregator
    {
        private readonly IReadOnlyCollection<Type> _supportedEvents;
        protected readonly IList<IEvent> AggregatedEvents;

        public EventAggregator(IList<IEvent> aggregatedEvents)
        {
            _supportedEvents = GetType().GenericTypeArguments;

            if (!_supportedEvents.Any())
            {
                throw new NotSupportedException();
            }

            AggregatedEvents = aggregatedEvents;
        }

        public bool IsSupport(Type eventType)
        {
            return _supportedEvents.Any(x => x == eventType);
        }

        public void ResetAggregatedEvents()
        {
            AggregatedEvents.Clear();
        }

        public IReadOnlyCollection<IEvent> GetAggregatedEvents()
        {
            return AggregatedEvents.ToList();
        }

        public bool Aggregate(IEvent @event)
        {
            AggregatedEvents.Add(@event);

            if (AggregatedEvents.Select(x => x.GetType()).Count(_supportedEvents.Contains) != _supportedEvents.Count)
            {
                return false;
            }

            InvokeGenericEventHandler();
            ResetAggregatedEvents();

            return true;
        }

        protected abstract void InvokeGenericEventHandler();
    }
}