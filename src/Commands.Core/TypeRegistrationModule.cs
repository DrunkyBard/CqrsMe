using Autofac;

namespace Commands.Core
{
    public class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacCommandHandlerFactory>()
                .As<ICommandHandlerFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandComposition>()
                .As<ICommandComposition>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof (MapCommandHandler<,>))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterType<AutomapperObjectMapper>()
                .As<IObjectMapper>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
