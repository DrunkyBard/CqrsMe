using System;
using Cqrs.Messaging.Core;

namespace Cqrs.Messaging.Rabbitmq
{
    public sealed class E<TCommand> : E//TODO: Name: CommandEnvelopeDto
    {



        public static implicit operator Envelope<TCommand>(E<TCommand> contract)
        {
            return new Envelope<TCommand>(contract.MessageId, (TCommand)contract.Body, contract.IsRedelivered);
        }
    }

    public abstract class E
    {
        public Guid MessageId { get; set; }

        public bool IsRedelivered { get; set; }

        public object Body { get; set; }
    }
}
