using Cqrs.Domain.Core;

namespace Cqrs.Domain.ProcessManager
{
    public class AggregatedEvent<TState> where TState : struct
    {
        public readonly TState PermittedState;
        public readonly IEvent Event;

        public AggregatedEvent(TState permittedState, IEvent @event)
        {
            PermittedState = permittedState;
            Event = @event;
        }
    }
}