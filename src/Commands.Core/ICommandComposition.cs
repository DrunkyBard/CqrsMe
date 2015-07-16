namespace Commands.Core
{
    public interface ICommandComposition
    {
        ICommandChain StartWith<TCommand>(TCommand command) where TCommand : ICommand;

        ICommandChain<TOutput> StartWith<TCommand, TOutput>(TCommand command) where TCommand : ICommand;
    }
}
