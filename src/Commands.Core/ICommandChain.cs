using System;

namespace Commands.Core
{
    public interface ICommandChain : ICommandRunner
    {
        ICommandChain ContinueWith<TCommand>(TCommand command) where TCommand : ICommand;

        ICommandChain<TOutput> ContinueWith<TCommand, TOutput>(TCommand command) where TCommand : ICommand;
    }

    public interface ICommandChain<out TOutput> : ICommandRunner<TOutput>
    {
        ICommandChain<TContinuationOutput> ContinueWith<TCommand, TContinuationOutput>() where TCommand : ICommand;

        ICommandChain<TContinuationOutput> ContinueWith<TCommand, TContinuationOutput>(Func<TOutput, TCommand> continuationMapping) where TCommand : ICommand;
        
        ICommandChain ContinueWith<TCommand>() where TCommand : ICommand;

        ICommandChain ContinueWith<TCommand>(Func<TOutput, TCommand> continuationMapping) where TCommand : ICommand;
    }
}
