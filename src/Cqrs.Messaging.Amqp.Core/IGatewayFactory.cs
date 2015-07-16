namespace Cqrs.Messaging.Configuration.Amqp
{
    public interface IGatewayFactory
    {
        IGateway Create(GatewayConfiguration configuration);
    }
}
