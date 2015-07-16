using Autofac;
using Commands.Core;

namespace Cqrs.Messaging.Core
{
    public class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}
