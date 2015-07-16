using System;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public sealed class EventMetadata
    {
        public readonly Guid DispatchedMessageId;

        public readonly string Type;

        public EventMetadata(Guid dispatchedMessageId, string type)
        {
            DispatchedMessageId = dispatchedMessageId;
            Type = type;
        }
    }
}
