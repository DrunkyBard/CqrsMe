using Autofac;
using Cqrs.Messaging.Configuration.Amqp;
using Cqrs.Messaging.Core;

namespace Cqrs.Messaging.Rabbitmq
{
    public sealed class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitMqEventBus>()
                .As<IEventBus>()
                .SingleInstance();

            builder.RegisterType<RabbitMqCommandBus>()
                .As<ICommandBus>()
                .SingleInstance();
            
            builder.RegisterType<RabbitMqGatewayFactory>()
                .As<IGatewayFactory>()
                .InstancePerDependency();
        }
    }
}
