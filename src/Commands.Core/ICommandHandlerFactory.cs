namespace Commands.Core
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler<TCommand> Create<TCommand>() where TCommand : ICommand;

        ICommandHandler<TCommand, TOutput> Create<TCommand, TOutput>() where TCommand : ICommand;
    }
}
