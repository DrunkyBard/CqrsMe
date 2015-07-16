using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Commands.Core;
using Cqrs.Domain.Core;
using Cqrs.Domain.ProcessManager.Exceptions;

namespace Cqrs.Domain.ProcessManager
{
    public abstract class ProcessManager<TState> // TODO: Move ProcessManager to Application layer
        where TState : struct
    {
        internal TState CurrentState;
        private readonly IReadOnlyCollection<AggregatedEvent<TState>> _aggregatedEvents;

        private readonly Action<TState> _conditionalAction;
        private readonly Queue<ICommand> _undispatchedCommands;
        internal readonly IList<ProcessManagerConfiguration<TState>> ProcessManagerConfigurations;

        public ProcessManager(TState initialState)
            : this(initialState, new List<AggregatedEvent<TState>>())
        {}

        public ProcessManager(TState initialState, IReadOnlyCollection<AggregatedEvent<TState>> aggregatedEvents)
        {
            Contract.Requires(aggregatedEvents != null, "AggregatedEvents parameter should not be null.");

            CurrentState = initialState;
            _aggregatedEvents = aggregatedEvents;
            _conditionalAction = expectedState =>
            {
                if (!CurrentState.Equals(expectedState))
                {
                    throw new InconsistentStateException(string.Format("Inconsistent state. Current state: {0}. Expected state: {1}", CurrentState, expectedState));
                }
            };
            ProcessManagerConfigurations = new List<ProcessManagerConfiguration<TState>>();
            _undispatchedCommands = new Queue<ICommand>();
        }

        protected void Dispatch(ICommand command)
        {
            _undispatchedCommands.Enqueue(command);
        }

        public IReadOnlyCollection<ICommand> GetUndispatchedCommands()
        {
            return _undispatchedCommands.ToList();
        }

        protected ConditionalProcessManagerConfiguration<TState> IfIn(TState conditionalState)
        {
            var filteredByPermittedStateEvents = _aggregatedEvents.Where(x => x.PermittedState.Equals(conditionalState)).ToList();
            var processManagerConfiguration = new ProcessManagerConfiguration<TState>
            {
                ConditionalAction = () => _conditionalAction(conditionalState), 
                PermittedState = conditionalState
            };

            return new ConditionalProcessManagerConfiguration<TState>(processManagerConfiguration, this, filteredByPermittedStateEvents);
        }

        public void Raise<TEvent>(TEvent @event) where TEvent : IEvent 
        {
            var configuration = ProcessManagerConfigurations
                .SingleOrDefault(x => CurrentState.Equals(x.PermittedState) && x.EventAggregator.IsSupport(typeof(TEvent)));

            if (configuration == null)
            {
                throw new EventHandlerNotFoundException(string.Format("Event handler not found for {0} state", CurrentState));
            }

            configuration.Transition(@event);
        }

        public ProcessManagerSnapshot<TState> GetSnapshot()
        {
            var aggregatedEvents = new List<AggregatedEvent<TState>>();

            foreach (var processManagerConfiguration in ProcessManagerConfigurations.Where(x => x.PermittedState.Equals(CurrentState)))
            {
                aggregatedEvents.AddRange(processManagerConfiguration.EventAggregator.GetAggregatedEvents()
                    .Select(x => new AggregatedEvent<TState>(processManagerConfiguration.PermittedState, x))
                    .ToList());
            }

            return new ProcessManagerSnapshot<TState>(CurrentState, aggregatedEvents);
        }
    }
}