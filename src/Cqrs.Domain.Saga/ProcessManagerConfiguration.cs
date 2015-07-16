using System;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.ProcessManager
{
    public class ProcessManagerConfiguration<TState> where TState : struct 
    {
        public TState PermittedState { get; set; }

        public Action<IEvent> Transition { get; set; }

        public Action ConditionalAction { get; set; }

        public EventAggregator EventAggregator { get; set; }
    }
}