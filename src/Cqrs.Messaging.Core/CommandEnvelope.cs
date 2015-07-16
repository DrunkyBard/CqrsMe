using System;
using Commands.Core;
using Cqrs.Common;
using Cqrs.Domain.Core;

namespace Cqrs.Messaging.Core
{
    public sealed class Envelope<TBody> : ICommand, IEvent
    {
        public readonly Guid MessageId;

        public bool IsRedelivered;

        public readonly TBody Body;

        public Envelope(TBody body) : this(body.ToGuid(), body, false)
        {}

        public Envelope(Guid messageId, TBody body, bool isRedelivered)
        {
            MessageId = messageId;
            Body = body;
            IsRedelivered = isRedelivered;
        }
    }
}
