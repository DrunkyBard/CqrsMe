using Cqrs.Domain.Core;

namespace Cqrs.Messaging.Configuration.Amqp
{
    public interface IGatewayEventsConfigurator
    {
        IGatewayEventsConfigurator Subscribe<TEvent>() where TEvent : IEvent;
    }
}
