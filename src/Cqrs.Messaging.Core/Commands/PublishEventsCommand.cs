using System.Collections.Generic;
using Commands.Core;
using Cqrs.Domain.Core;

namespace Cqrs.Messaging.Core.Commands
{
    public sealed class PublishEventsCommand : ICommand
    {
        public readonly IReadOnlyCollection<IEvent> Events;

        public PublishEventsCommand(IReadOnlyCollection<IEvent> events)
        {
            Events = events;
        }
    }
}
