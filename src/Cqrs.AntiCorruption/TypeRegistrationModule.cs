using Autofac;

namespace Cqrs.AntiCorruption
{
    public sealed class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AntiCorruption>()
                .As<IAntiCorruption>()
                .InstancePerDependency();
        }
    }
}
