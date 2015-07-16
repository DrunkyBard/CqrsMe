using System.Collections.Generic;

namespace Cqrs.Domain.ProcessManager
{
    public class ProcessManagerSnapshot<TState> where TState : struct 
    {
        public readonly TState CurrentState;
        public readonly IReadOnlyCollection<AggregatedEvent<TState>> AggregatedEvents;

        public ProcessManagerSnapshot(TState currentState, IReadOnlyCollection<AggregatedEvent<TState>> aggregatedEvents)
        {
            CurrentState = currentState;
            AggregatedEvents = aggregatedEvents;
        }
    }
}