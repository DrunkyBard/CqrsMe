using System;
using System.Linq;
using Cqrs.Domain.Core;
using Cqrs.Domain.Eventing;

namespace Cqrs.Projection.Core
{
    public sealed class AggregateSubscriberConfigurationBuilder
    {
        private readonly ProjectionGatewayConfigurationBuilder _projectionGatewayConfigurationBuilder;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly string _aggregateRoot;

        internal AggregateSubscriberConfigurationBuilder(ProjectionGatewayConfigurationBuilder projectionGatewayConfigurationBuilder, IEventHandlerFactory eventHandlerFactory, string aggregateRoot)
        {
            _projectionGatewayConfigurationBuilder = projectionGatewayConfigurationBuilder;
            _eventHandlerFactory = eventHandlerFactory;
            _aggregateRoot = aggregateRoot;
        }

        public AggregateSubscriberConfigurationBuilder SubscribeTo<TEvent>() where TEvent : IEvent
        {
            if (!_projectionGatewayConfigurationBuilder.Subscriptions.ContainsKey(_aggregateRoot) || !_projectionGatewayConfigurationBuilder.Subscriptions[_aggregateRoot].Any(x => x.EventType == typeof(TEvent)))
            {
                Action<object> eventHandler = @event =>
                {
                    var handlers = _eventHandlerFactory.Create<TEvent>();

                    foreach (var domainEventHandler in handlers)
                    {
                        domainEventHandler.Handle((TEvent)@event);
                    }
                };
                _projectionGatewayConfigurationBuilder.Subscriptions.Add(_aggregateRoot, new Subscription(typeof(TEvent), eventHandler));
            }

            return this;
        }

        public AggregateSubscriberConfigurationBuilder For(string aggregateRoot)
        {
            return _projectionGatewayConfigurationBuilder.For(aggregateRoot);
        }

        public ProjectionGatewayConfiguration Build()
        {
            return _projectionGatewayConfigurationBuilder.Build();
        }
    }
}
