namespace Commands.Core
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Execute(TCommand command);
    }

    public interface ICommandHandler<in TCommand, out TResult>
    {
        TResult Execute(TCommand command);
    }
}
