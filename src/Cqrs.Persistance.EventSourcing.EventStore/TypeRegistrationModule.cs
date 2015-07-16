using Autofac;
using Cqrs.Persistance.EventSourcing.Core;

namespace Cqrs.Persistance.EventSourcing.EventStore
{
    public sealed class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<EventStoreDataContext>()
                .As<IEventStoreContext>()
                .SingleInstance();
        }
    }
}
