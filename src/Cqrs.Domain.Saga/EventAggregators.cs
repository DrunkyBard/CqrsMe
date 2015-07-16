using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.ProcessManager
{
    public class SingleEventAggregator<TEvent> : EventAggregator
        where TEvent : IEvent
    {
        private readonly Action<TEvent> _eventHandler;

        public SingleEventAggregator(IList<IEvent> aggregatedEvents, Action<TEvent> eventHandler)
            : base(aggregatedEvents)
        {
            _eventHandler = eventHandler;
        }

        protected override void InvokeGenericEventHandler()
        {
            var aggregatedEvent = (TEvent)AggregatedEvents.Single(x => x.GetType() == typeof(TEvent));
            _eventHandler(aggregatedEvent);
        }
    }

    public class DoubleEventAggregator<TEvent1, TEvent2> : EventAggregator
        where TEvent1 : IEvent
        where TEvent2 : IEvent
    {
        private readonly Action<TEvent1, TEvent2> _eventHandler;

        public DoubleEventAggregator(IList<IEvent> aggregatedEvents, Action<TEvent1, TEvent2> eventHandler)
            : base(aggregatedEvents)
        {
            _eventHandler = eventHandler;
        }

        protected override void InvokeGenericEventHandler()
        {
            var aggregatedEvent1 = (TEvent1)AggregatedEvents.First(x => x.GetType() == typeof(TEvent1));
            AggregatedEvents.Remove(aggregatedEvent1);
            var aggregatedEvent2 = (TEvent2)AggregatedEvents.First(x => x.GetType() == typeof(TEvent2));
            _eventHandler(aggregatedEvent1, aggregatedEvent2);
        }
    }
    
    public class TripleEventAggregator<TEvent1, TEvent2, TEvent3> : EventAggregator
        where TEvent1 : IEvent
        where TEvent2 : IEvent
        where TEvent3 : IEvent
    {
        private readonly Action<TEvent1, TEvent2, TEvent3> _eventHandler;

        public TripleEventAggregator(IList<IEvent> aggregatedEvents, Action<TEvent1, TEvent2, TEvent3> eventHandler)
            : base(aggregatedEvents)
        {
            _eventHandler = eventHandler;
        }

        protected override void InvokeGenericEventHandler()
        {
            var aggregatedEvent1 = (TEvent1)AggregatedEvents.First(x => x.GetType() == typeof(TEvent1));
            AggregatedEvents.Remove(aggregatedEvent1);
            var aggregatedEvent2 = (TEvent2)AggregatedEvents.First(x => x.GetType() == typeof(TEvent2));
            AggregatedEvents.Remove(aggregatedEvent2);
            var aggregatedEvent3 = (TEvent3)AggregatedEvents.First(x => x.GetType() == typeof(TEvent3));
            AggregatedEvents.Remove(aggregatedEvent3);
            
            _eventHandler(aggregatedEvent1, aggregatedEvent2, aggregatedEvent3);
        }
    }
    
    public class QuadrupleEventAggregator<TEvent1, TEvent2, TEvent3, TEvent4> : EventAggregator
        where TEvent1 : IEvent
        where TEvent2 : IEvent
        where TEvent3 : IEvent
        where TEvent4 : IEvent
    {
        private readonly Action<TEvent1, TEvent2, TEvent3, TEvent4> _eventHandler;

        public QuadrupleEventAggregator(IList<IEvent> aggregatedEvents, Action<TEvent1, TEvent2, TEvent3, TEvent4> eventHandler)
            : base(aggregatedEvents)
        {
            _eventHandler = eventHandler;
        }

        protected override void InvokeGenericEventHandler()
        {
            var aggregatedEvent1 = (TEvent1)AggregatedEvents.First(x => x.GetType() == typeof(TEvent1));
            AggregatedEvents.Remove(aggregatedEvent1);
            var aggregatedEvent2 = (TEvent2)AggregatedEvents.First(x => x.GetType() == typeof(TEvent2));
            AggregatedEvents.Remove(aggregatedEvent2);
            var aggregatedEvent3 = (TEvent3)AggregatedEvents.First(x => x.GetType() == typeof(TEvent3));
            AggregatedEvents.Remove(aggregatedEvent3);
            var aggregatedEvent4 = (TEvent4)AggregatedEvents.First(x => x.GetType() == typeof(TEvent4));
            AggregatedEvents.Remove(aggregatedEvent4);
            
            _eventHandler(aggregatedEvent1, aggregatedEvent2, aggregatedEvent3, aggregatedEvent4);
        }
    }
    
    public class EventAggregator<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5> : EventAggregator
        where TEvent1 : IEvent
        where TEvent2 : IEvent
        where TEvent3 : IEvent
        where TEvent4 : IEvent
        where TEvent5 : IEvent
    {
        private readonly Action<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5> _eventHandler;

        public EventAggregator(IList<IEvent> aggregatedEvents, Action<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5> eventHandler)
            : base(aggregatedEvents)
        {
            _eventHandler = eventHandler;
        }

        protected override void InvokeGenericEventHandler()
        {
            var aggregatedEvent1 = (TEvent1)AggregatedEvents.First(x => x.GetType() == typeof(TEvent1));
            AggregatedEvents.Remove(aggregatedEvent1);
            var aggregatedEvent2 = (TEvent2)AggregatedEvents.First(x => x.GetType() == typeof(TEvent2));
            AggregatedEvents.Remove(aggregatedEvent2);
            var aggregatedEvent3 = (TEvent3)AggregatedEvents.First(x => x.GetType() == typeof(TEvent3));
            AggregatedEvents.Remove(aggregatedEvent3);
            var aggregatedEvent4 = (TEvent4)AggregatedEvents.First(x => x.GetType() == typeof(TEvent4));
            AggregatedEvents.Remove(aggregatedEvent4);
            var aggregatedEvent5 = (TEvent5)AggregatedEvents.First(x => x.GetType() == typeof(TEvent5));
            AggregatedEvents.Remove(aggregatedEvent5);
            
            _eventHandler(aggregatedEvent1, aggregatedEvent2, aggregatedEvent3, aggregatedEvent4, aggregatedEvent5);
        }
    }
    
    public class EventAggregator<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5, TEvent6> : EventAggregator
        where TEvent1 : IEvent
        where TEvent2 : IEvent
        where TEvent3 : IEvent
        where TEvent4 : IEvent
        where TEvent5 : IEvent
        where TEvent6 : IEvent
    {
        private readonly Action<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5, TEvent6> _eventHandler;

        public EventAggregator(IList<IEvent> aggregatedEvents, Action<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5, TEvent6> eventHandler)
            : base(aggregatedEvents)
        {
            _eventHandler = eventHandler;
        }

        protected override void InvokeGenericEventHandler()
        {
            var aggregatedEvent1 = (TEvent1)AggregatedEvents.First(x => x.GetType() == typeof(TEvent1));
            AggregatedEvents.Remove(aggregatedEvent1);
            var aggregatedEvent2 = (TEvent2)AggregatedEvents.First(x => x.GetType() == typeof(TEvent2));
            AggregatedEvents.Remove(aggregatedEvent2);
            var aggregatedEvent3 = (TEvent3)AggregatedEvents.First(x => x.GetType() == typeof(TEvent3));
            AggregatedEvents.Remove(aggregatedEvent3);
            var aggregatedEvent4 = (TEvent4)AggregatedEvents.First(x => x.GetType() == typeof(TEvent4));
            AggregatedEvents.Remove(aggregatedEvent4);
            var aggregatedEvent5 = (TEvent5)AggregatedEvents.First(x => x.GetType() == typeof(TEvent5));
            AggregatedEvents.Remove(aggregatedEvent5);
            var aggregatedEvent6 = (TEvent6)AggregatedEvents.First(x => x.GetType() == typeof(TEvent6));
            AggregatedEvents.Remove(aggregatedEvent6);
            
            _eventHandler(aggregatedEvent1, aggregatedEvent2, aggregatedEvent3, aggregatedEvent4, aggregatedEvent5, aggregatedEvent6);
        }
    }
}