using Commands.Core;
using Cqrs.Messaging.Core.Commands;

namespace Cqrs.Messaging.Core.CommandHandlers
{
    internal sealed class PublishEventsCommandHandler : ICommandHandler<PublishEventsCommand>
    {
        private readonly IEventBus _eventBus;

        public PublishEventsCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Execute(PublishEventsCommand command)
        {
            _eventBus.Publish(command.Events);
        }
    }
}
