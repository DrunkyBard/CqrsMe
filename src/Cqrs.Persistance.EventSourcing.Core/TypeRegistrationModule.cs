using Autofac;
using Commands.Core;
using Cqrs.Persistance.EventSourcing.Core.CommandHandlers;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public sealed class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.IsClosedTypeOf(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof (PersistEventsCommandHandler<>))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof (PersistSnapshotCommandHandler<>))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof (RestoreEventSourcedAggregateCommandHandler<,,>))
                .As(typeof(RestoreEventSourcedAggregateCommandHandler<,,>).GetInterfaces())
                .InstancePerDependency();
        }
    }
}
