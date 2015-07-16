namespace Cqrs.Messaging.Configuration.Amqp
{
    public interface IGateway
    {
        IGatewayEventsConfigurator ForEvents();

        IGatewayCommandsConfigurator ForCommands();
    }
}
