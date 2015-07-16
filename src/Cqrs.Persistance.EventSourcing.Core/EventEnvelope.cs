using Cqrs.Domain.Core;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public sealed class EventEnvelope
    {
        public readonly IEvent Event;
        public readonly EventMetadata Metadata;

        public EventEnvelope(IEvent @event, EventMetadata metadata)
        {
            Event = @event;
            this.Metadata = metadata;
        }
    }
}
