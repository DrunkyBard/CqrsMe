using System;
using System.Collections.Generic;

namespace Cqrs.Persistance.MessageStore
{
    public sealed class DispatchedMessage
    {
        public readonly Guid Id;
        public readonly bool IsDispatched;
        public readonly IReadOnlyCollection<object> Payload; 


        public DispatchedMessage(Guid id, bool isDispatched, IReadOnlyCollection<object> payload)
        {
            Id = id;
            IsDispatched = isDispatched;
            Payload = payload;
        }
    }
}
