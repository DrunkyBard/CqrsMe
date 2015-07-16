using Cqrs.Domain.Core;
using Cqrs.Messaging.Configuration.Amqp;
using Cqrs.Messaging.Core;

namespace Cqrs.Messaging.Rabbitmq
{
    internal sealed class RabbitMqGatewayEventsConfigurator : IGatewayEventsConfigurator
    {
        private readonly IEventBus _eventBus;
        private readonly int _eventSubscriberThreadCount;

        public RabbitMqGatewayEventsConfigurator(IEventBus eventBus, int eventSubscriberThreadCount)
        {
            _eventBus = eventBus;
            _eventSubscriberThreadCount = eventSubscriberThreadCount;
        }

        public IGatewayEventsConfigurator Subscribe<TEvent>() where TEvent : IEvent
        {
            for (int i = 0; i < _eventSubscriberThreadCount; i++)
            {
                _eventBus.Subscribe<TEvent>();
            }

            return this;
        }
    }
}
