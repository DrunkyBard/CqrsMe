using System;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.ProcessManager
{
    public class TransitionalProcessManagerConfiguration<TState> where TState : struct
    {
        private readonly ProcessManagerConfiguration<TState> _processManagerConfiguration;
        private readonly ProcessManager<TState> _originProcessManager;

        public TransitionalProcessManagerConfiguration(ProcessManagerConfiguration<TState> processManagerConfiguration, ProcessManager<TState> originProcessManager)
        {
            _processManagerConfiguration = processManagerConfiguration;
            _originProcessManager = originProcessManager;
        }

        public void AfterTransitionTo(TState transitionState)
        {
            Action<IEvent> transitionalAction = @event =>
            {
                _processManagerConfiguration.ConditionalAction();
                var isFullyAggregate = _processManagerConfiguration.EventAggregator.Aggregate(@event);

                if (isFullyAggregate)
                {
                    _originProcessManager.CurrentState = transitionState;

                    foreach (var processManagerConfiguration in _originProcessManager.ProcessManagerConfigurations)
                    {
                        processManagerConfiguration.EventAggregator.ResetAggregatedEvents();
                    }
                }
            };

            _processManagerConfiguration.Transition = transitionalAction;

            _originProcessManager.ProcessManagerConfigurations.Add(_processManagerConfiguration);
        }
    }
}