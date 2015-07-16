using Autofac;

namespace Cqrs.ObjectMapping
{
    public sealed class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutomapperObjectMapper>()
                .As<IObjectMapper>()
                .InstancePerDependency();
        }
    }
}
