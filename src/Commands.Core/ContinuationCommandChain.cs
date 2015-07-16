using System;

namespace Commands.Core
{
    internal sealed class ContinuationCommandChain : ICommandChain
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly Action _commandDelegate;
        private readonly IObjectMapper _objectMapper;

        public ContinuationCommandChain(ICommandHandlerFactory commandHandlerFactory, IObjectMapper objectMapper, Action commandDelegate)
        {
            _commandHandlerFactory = commandHandlerFactory;
            _commandDelegate = commandDelegate;
            _objectMapper = objectMapper;
        }

        void ICommandRunner.Run()
        {
            _commandDelegate();
        }

        public ICommandChain ContinueWith<TContinuationCommand>(TContinuationCommand command) where TContinuationCommand : ICommand
        {
            var continuationCommandHandler = _commandHandlerFactory.Create<TContinuationCommand>();

            return new ContinuationCommandChain(_commandHandlerFactory, _objectMapper, () =>
            {
                _commandDelegate();
                continuationCommandHandler.Execute(command);
            });
        }

        public ICommandChain<TOutput> ContinueWith<TContinuationCommand, TOutput>(TContinuationCommand command) 
            where TContinuationCommand : ICommand
        {
            var continuationCommandHandler = _commandHandlerFactory.Create<TContinuationCommand, TOutput>();

            return new ContinuationCommandChain<TOutput>(_commandHandlerFactory, _objectMapper, () =>
            {
                _commandDelegate();
                return continuationCommandHandler.Execute(command);
            });
        }
    }

    internal sealed class ContinuationCommandChain<TOutput> : ICommandChain<TOutput>
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly Func<TOutput> _commandDelegate;
        private readonly IObjectMapper _objectMapper;

        public ContinuationCommandChain(ICommandHandlerFactory commandHandlerFactory, IObjectMapper objectMapper, Func<TOutput> commandDelegate)
        {
            _commandHandlerFactory = commandHandlerFactory;
            _commandDelegate = commandDelegate;
            _objectMapper = objectMapper;
        }

        public TOutput Run()
        {
            return _commandDelegate();
        }

        public ICommandChain<TContinuationOutput> ContinueWith<TCommand, TContinuationOutput>(Func<TOutput, TCommand> continuationMapping) where TCommand : ICommand
        {
            var continuationCommandHandler = _commandHandlerFactory.Create<TCommand, TContinuationOutput>();

            return new ContinuationCommandChain<TContinuationOutput>(_commandHandlerFactory, _objectMapper, () => continuationCommandHandler.Execute(continuationMapping(_commandDelegate())));
        }

        public ICommandChain ContinueWith<TContinuationCommand>() where TContinuationCommand : ICommand
        {
            var continuationCommandHandler = _commandHandlerFactory.Create<TContinuationCommand>();
            var mapCommandHandler = _commandHandlerFactory.Create<MapCommand<TOutput, TContinuationCommand>, TContinuationCommand>();
            var mapCommand = new MapCommand<TOutput, TContinuationCommand>(_commandDelegate);

            return new ContinuationCommandChain(_commandHandlerFactory, _objectMapper, () => continuationCommandHandler.Execute(mapCommandHandler.Execute(mapCommand)));
        }

        public ICommandChain ContinueWith<TCommand>(Func<TOutput, TCommand> continuationMapping) where TCommand : ICommand
        {
            var continuationCommandHandler = _commandHandlerFactory.Create<TCommand>();

            return new ContinuationCommandChain(_commandHandlerFactory, _objectMapper, () => continuationCommandHandler.Execute(continuationMapping(_commandDelegate())));
        }

        public ICommandChain<TContinuationOutput> ContinueWith<TContinuationCommand, TContinuationOutput>()
            where TContinuationCommand : ICommand
        {
            var continuationCommandHandler = _commandHandlerFactory.Create<TContinuationCommand, TContinuationOutput>();
            var mapCommandHandler = _commandHandlerFactory.Create<MapCommand<TOutput, TContinuationCommand>, TContinuationCommand>();
            var mapCommand = new MapCommand<TOutput, TContinuationCommand>(_commandDelegate);

            return new ContinuationCommandChain<TContinuationOutput>(_commandHandlerFactory, _objectMapper, () => continuationCommandHandler.Execute(mapCommandHandler.Execute(mapCommand)));
        }
    }
}
