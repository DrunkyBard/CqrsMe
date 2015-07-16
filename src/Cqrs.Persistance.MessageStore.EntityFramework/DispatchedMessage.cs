using System;

namespace Cqrs.Persistance.MessageStore.EntityFramework
{
    internal sealed class DispatchedMessageEntity
    {
        public Guid Id { get; set; }

        public byte[] Payload { get; set; }

        public bool IsDispatched { get; set; }
    }
}
