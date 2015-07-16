using System;
using Commands.Core;
using Cqrs.Messaging.Configuration.Amqp;
using EasyNetQ;
using EasyNetQ.Consumer;

namespace Cqrs.Messaging.Rabbitmq
{
    internal sealed class RabbitMqGateway : IGateway
    {
        private int _eventSubscriberThreadCount;
        private int _commandHandlerThreadCount;
        private readonly Core.IEventBus _eventBus;
        private readonly IBus _externalMessageBus;
        private readonly ICommandHandlerFactory _commandHandlerFactory;

        public RabbitMqGateway(string connectionString, int eventSubscriberThreadCount, int commandHandlerThreadCount, ILocator locator)
        {
            _eventSubscriberThreadCount = eventSubscriberThreadCount;
            _commandHandlerThreadCount = commandHandlerThreadCount;
            var adapter = new EasyNetQContainerAdapter(locator);
            RabbitHutch.SetContainerFactory(() => adapter);
            _externalMessageBus = RabbitHutch.CreateBus(connectionString, register => register.Register<IConsumerErrorStrategy, MyClass>());
            _eventBus = locator.Resolve<Core.IEventBus>();
            _commandHandlerFactory = locator.Resolve<ICommandHandlerFactory>();
            var a = locator.Resolve<IConsumerErrorStrategy>();
        }

        public IGatewayEventsConfigurator ForEvents()
        {
            return new RabbitMqGatewayEventsConfigurator(_eventBus, _eventSubscriberThreadCount);
        }

        public IGatewayCommandsConfigurator ForCommands()
        {
           return new RabbitMqGatewayCommandsConfigurator(_externalMessageBus, _commandHandlerFactory, _commandHandlerThreadCount);
        }
    }

    class MyClass : IConsumerErrorStrategy
    {
        public void Dispose()
        {
            var a = 1;
        }

        public AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
        {
            return AckStrategies.NackWithRequeue;
        }

        public AckStrategy HandleConsumerCancelled(ConsumerExecutionContext context)
        {
            return AckStrategies.NackWithRequeue;
        }
    }
}
