using System.Reflection;
using System.Threading.Tasks;
using Commands.Core;
using Cqrs.Messaging.Configuration.Amqp;
using Cqrs.Messaging.Core;
using EasyNetQ;
using EasyNetQ.NonGeneric;

namespace Cqrs.Messaging.Rabbitmq
{
    internal sealed class RabbitMqGatewayCommandsConfigurator : IGatewayCommandsConfigurator
    {
        private readonly IBus _bus;
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly int _commandHandlerThreadCount;

        public RabbitMqGatewayCommandsConfigurator(IBus bus, ICommandHandlerFactory commandHandlerFactory, int commandHandlerThreadCount)
        {
            _bus = bus;
            _commandHandlerFactory = commandHandlerFactory;
            _commandHandlerThreadCount = commandHandlerThreadCount;
        }

        public IGatewayCommandsConfigurator Handle<TCommand>() where TCommand : ICommand
        {
            for (int i = 0; i < _commandHandlerThreadCount; i++)
            {
                _bus.SubscribeAsync(
                    typeof(E<TCommand>),
                    Assembly.GetEntryAssembly().GetName().Name,
                    incomingCommand => Task.Factory.StartNew(() =>
                    {
                        Envelope<TCommand> command = (E<TCommand>)incomingCommand;
                        var commandHandler = _commandHandlerFactory.Create<Envelope<TCommand>>();
                        commandHandler.Execute(command);
                    }));
            }

            return this;
        }
    }
}
