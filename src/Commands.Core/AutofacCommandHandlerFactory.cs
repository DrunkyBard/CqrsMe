using Autofac;

namespace Commands.Core
{
    internal class AutofacCommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacCommandHandlerFactory(ILifetimeScope parentScope)
        {
            _lifetimeScope = parentScope;
        }

        public ICommandHandler<TCommand> Create<TCommand>() where TCommand : ICommand
        {
            var commandHandler = _lifetimeScope.Resolve<ICommandHandler<TCommand>>();

            return commandHandler;
        }

        public ICommandHandler<TCommand, TOutput> Create<TCommand, TOutput>() where TCommand : ICommand
        {
            var commandHandler = _lifetimeScope.Resolve<ICommandHandler<TCommand, TOutput>>();

            return commandHandler;
        }
    }
}
