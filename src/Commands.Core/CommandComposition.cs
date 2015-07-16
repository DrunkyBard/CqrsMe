namespace Commands.Core
{
    internal sealed class CommandComposition : ICommandComposition
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly IObjectMapper _objectMapper;

        public CommandComposition(ICommandHandlerFactory commandHandlerFactory, IObjectMapper objectMapper)
        {
            _commandHandlerFactory = commandHandlerFactory;
            _objectMapper = objectMapper;
        }

        public ICommandChain StartWith<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = _commandHandlerFactory.Create<TCommand>();

            return new ContinuationCommandChain(_commandHandlerFactory, _objectMapper, () => commandHandler.Execute(command));
        }

        public ICommandChain<TOutput> StartWith<TCommand, TOutput>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = _commandHandlerFactory.Create<TCommand, TOutput>();

            return new ContinuationCommandChain<TOutput>(_commandHandlerFactory, _objectMapper, () => commandHandler.Execute(command));
        }
    }
}
