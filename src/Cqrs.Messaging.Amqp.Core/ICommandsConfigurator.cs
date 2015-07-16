using Commands.Core;

namespace Cqrs.Messaging.Configuration.Amqp
{
    public interface IGatewayCommandsConfigurator
    {
        IGatewayCommandsConfigurator Handle<TCommand>() where TCommand : ICommand;
    }
}
