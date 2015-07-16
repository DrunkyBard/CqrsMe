using Autofac;

namespace Cqrs.Domain.Eventing
{
    public sealed class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacEventHandlerFactory>()
                .As<IEventHandlerFactory>()
                .InstancePerDependency();
        }
    }
}
