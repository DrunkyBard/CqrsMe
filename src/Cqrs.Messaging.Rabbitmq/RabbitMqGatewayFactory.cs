using Cqrs.Messaging.Configuration.Amqp;

namespace Cqrs.Messaging.Rabbitmq
{
    internal sealed class RabbitMqGatewayFactory : IGatewayFactory
    {
        public IGateway Create(GatewayConfiguration configuration)
        {
            var connectionString = configuration.ToConnectionString();

            return new RabbitMqGateway(connectionString, configuration.EventSubscriberThreadCount, configuration.CommandHandlerThreadCount, configuration.Locator);
        }
    }
}
