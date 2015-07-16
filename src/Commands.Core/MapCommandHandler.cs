namespace Commands.Core
{
    internal sealed class MapCommandHandler<TSource, TDestinationCommand> : ICommandHandler<MapCommand<TSource, TDestinationCommand>, TDestinationCommand> 
        where TDestinationCommand : ICommand
    {
        private readonly IObjectMapper _objectMapper;

        public MapCommandHandler(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public TDestinationCommand Execute(MapCommand<TSource, TDestinationCommand> command)
        {
            var destinationCommand = _objectMapper.Map<TSource, TDestinationCommand>(command.Source());

            return destinationCommand;
        }
    }
}
