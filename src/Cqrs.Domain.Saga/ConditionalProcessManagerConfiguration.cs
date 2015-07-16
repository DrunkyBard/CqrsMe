using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Domain.Core;
using Cqrs.Domain.ProcessManager.Exceptions;

namespace Cqrs.Domain.ProcessManager
{
    public class ConditionalProcessManagerConfiguration<TState> where TState : struct
    {
        private readonly ProcessManagerConfiguration<TState> _processManagerConfiguration;
        private readonly ProcessManager<TState> _originProcessManager;
        private readonly IReadOnlyCollection<AggregatedEvent<TState>> _aggregatedEvents;

        public ConditionalProcessManagerConfiguration(ProcessManagerConfiguration<TState> processManagerConfiguration, ProcessManager<TState> originProcessManager, IReadOnlyCollection<AggregatedEvent<TState>> aggregatedEvents)
        {
            _processManagerConfiguration = processManagerConfiguration;
            _originProcessManager = originProcessManager;
            _aggregatedEvents = aggregatedEvents;
        }

        public TransitionalProcessManagerConfiguration<TState> ThenHandle<TEvent>(Action<TEvent> eventHandler) where TEvent : IEvent
        {
            IsAlreadyRegisteredGuard(typeof(TEvent));

            var events = _aggregatedEvents.Where(x => x.Event.GetType() == typeof(TEvent))
                .Select(x => x.Event)
                .ToList();
            var singleEventAggregator = new SingleEventAggregator<TEvent>(events, eventHandler);
            _processManagerConfiguration.EventAggregator = singleEventAggregator;

            return new TransitionalProcessManagerConfiguration<TState>(_processManagerConfiguration, _originProcessManager);
        }

        public TransitionalProcessManagerConfiguration<TState> WaitAll<TEvent1, TEvent2>(Action<TEvent1, TEvent2> eventHandler) 
            where TEvent1 : IEvent
            where TEvent2 : IEvent
        {
            IsAlreadyRegisteredGuard(typeof(TEvent1), typeof(TEvent2));

            var events = _aggregatedEvents.Where(x => x.Event.GetType() == typeof(TEvent1) || x.Event.GetType() == typeof(TEvent2))
                .Select(x => x.Event)
                .ToList();
            var doubleEventAggregator = new DoubleEventAggregator<TEvent1, TEvent2>(events, eventHandler);
            _processManagerConfiguration.EventAggregator = doubleEventAggregator;

            return new TransitionalProcessManagerConfiguration<TState>(_processManagerConfiguration, _originProcessManager);
        }

        public TransitionalProcessManagerConfiguration<TState> WaitAll<TEvent1, TEvent2, TEvent3>(Action<TEvent1, TEvent2, TEvent3> eventHandler)
            where TEvent1 : IEvent
            where TEvent2 : IEvent
            where TEvent3 : IEvent
        {
            IsAlreadyRegisteredGuard(typeof(TEvent1), typeof(TEvent2), typeof(TEvent3));

            var events = _aggregatedEvents.Where(x => x.Event.GetType() == typeof(TEvent1) || x.Event.GetType() == typeof(TEvent2) || x.Event.GetType() == typeof(TEvent3))
                .Select(x => x.Event)
                .ToList();
            var doubleEventAggregator = new TripleEventAggregator<TEvent1, TEvent2, TEvent3>(events, eventHandler);
            _processManagerConfiguration.EventAggregator = doubleEventAggregator;

            return new TransitionalProcessManagerConfiguration<TState>(_processManagerConfiguration, _originProcessManager);
        }
        
        public TransitionalProcessManagerConfiguration<TState> WaitAll<TEvent1, TEvent2, TEvent3, TEvent4>(Action<TEvent1, TEvent2, TEvent3, TEvent4> eventHandler)
            where TEvent1 : IEvent
            where TEvent2 : IEvent
            where TEvent3 : IEvent
            where TEvent4 : IEvent
        {
            IsAlreadyRegisteredGuard(typeof(TEvent1), typeof(TEvent2), typeof(TEvent3), typeof(TEvent4));

            var events = _aggregatedEvents.Where(x => x.Event.GetType() == typeof(TEvent1) || x.Event.GetType() == typeof(TEvent2) || x.Event.GetType() == typeof(TEvent3) || x.Event.GetType() == typeof(TEvent4))
                .Select(x => x.Event)
                .ToList();
            var doubleEventAggregator = new QuadrupleEventAggregator<TEvent1, TEvent2, TEvent3, TEvent4>(events, eventHandler);
            _processManagerConfiguration.EventAggregator = doubleEventAggregator;

            return new TransitionalProcessManagerConfiguration<TState>(_processManagerConfiguration, _originProcessManager);
        }

        public TransitionalProcessManagerConfiguration<TState> WaitAll<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5>(Action<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5> eventHandler)
            where TEvent1 : IEvent
            where TEvent2 : IEvent
            where TEvent3 : IEvent
            where TEvent4 : IEvent
            where TEvent5 : IEvent
        {
            IsAlreadyRegisteredGuard(typeof(TEvent1), typeof(TEvent2), typeof(TEvent3), typeof(TEvent4), typeof(TEvent5));

            var events = _aggregatedEvents.Where(x => x.Event.GetType() == typeof(TEvent1) || x.Event.GetType() == typeof(TEvent2) || x.Event.GetType() == typeof(TEvent3) || x.Event.GetType() == typeof(TEvent4) || x.Event.GetType() == typeof(TEvent5))
                .Select(x => x.Event)
                .ToList();
            var doubleEventAggregator = new EventAggregator<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5>(events, eventHandler);
            _processManagerConfiguration.EventAggregator = doubleEventAggregator;

            return new TransitionalProcessManagerConfiguration<TState>(_processManagerConfiguration, _originProcessManager);
        }

        public TransitionalProcessManagerConfiguration<TState> WaitAll<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5, TEvent6>(Action<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5, TEvent6> eventHandler)
            where TEvent1 : IEvent
            where TEvent2 : IEvent
            where TEvent3 : IEvent
            where TEvent4 : IEvent
            where TEvent5 : IEvent
            where TEvent6 : IEvent
        {
            IsAlreadyRegisteredGuard(typeof(TEvent1), typeof(TEvent2), typeof(TEvent3), typeof(TEvent4), typeof(TEvent5), typeof(TEvent6));

            var events = _aggregatedEvents.Where(x => x.Event.GetType() == typeof(TEvent1) || x.Event.GetType() == typeof(TEvent2) || x.Event.GetType() == typeof(TEvent3) || x.Event.GetType() == typeof(TEvent4) || x.Event.GetType() == typeof(TEvent5) || x.Event.GetType() == typeof(TEvent6))
                .Select(x => x.Event)
                .ToList();
            var doubleEventAggregator = new EventAggregator<TEvent1, TEvent2, TEvent3, TEvent4, TEvent5, TEvent6>(events, eventHandler);
            _processManagerConfiguration.EventAggregator = doubleEventAggregator;

            return new TransitionalProcessManagerConfiguration<TState>(_processManagerConfiguration, _originProcessManager);
        }

        private void IsAlreadyRegisteredGuard(params Type[] eventTypes)
        {
            var alreadyRegisteredEvents = eventTypes
                .Where(x => _originProcessManager.ProcessManagerConfigurations.Any(y => y.EventAggregator.IsSupport(x)))
                .ToList();
            var isContainsSameConfigurationForCurrentPermittedState = _originProcessManager.ProcessManagerConfigurations.Any(x => x.PermittedState.Equals(_processManagerConfiguration.PermittedState));
            var isContainsSameConfigurationForEventTypes = alreadyRegisteredEvents.Any();

            if (isContainsSameConfigurationForCurrentPermittedState && isContainsSameConfigurationForEventTypes)
            {
                var eventTypeNames = string.Join("; ", alreadyRegisteredEvents
                    .Select(x => x.FullName)
                    .ToList());

                throw new EventHandlerAlreadyRegisteredException(string.Format("For {0} state already registered next events: {1}", _processManagerConfiguration.PermittedState, eventTypeNames));
            }
        }
    }
}