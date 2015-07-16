using System.Diagnostics.Contracts;
using Commands.Core;
using Cqrs.Common;
using Cqrs.Messaging.Core;
using EasyNetQ;
using EasyNetQ.NonGeneric;
using JetBrains.Annotations;

namespace Cqrs.Messaging.Rabbitmq
{
    [UsedImplicitly]
    internal sealed class RabbitMqCommandBus : ICommandBus
    {
        private readonly IBus _bus;

        public RabbitMqCommandBus(IBus bus)
        {
            Contract.Requires(bus != null);
            _bus = bus;
        }

        public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
        {
            var messageEnvelope = new E<TCommand>
            {
                MessageId = command.ToGuid(),
                Body = command,
                IsRedelivered = false
            };
            _bus.Publish(typeof(E<TCommand>), messageEnvelope);
        }
    }
}
