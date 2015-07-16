using System.Linq;
using Commands.Core;
using Cqrs.Domain.Core;
using Cqrs.Persistance.EventSourcing.Core.Commands;

namespace Cqrs.Persistance.EventSourcing.Core.CommandHandlers
{
    internal sealed class PersistEventsCommandHandler<TIdentity> : ICommandHandler<PersistEventsCommand<TIdentity>> 
        where TIdentity : Identity
    {
        private readonly IEventStoreContext _eventStoreContext;

        public PersistEventsCommandHandler(IEventStoreContext eventStoreContext)
        {
            _eventStoreContext = eventStoreContext;
        }

        public void Execute(PersistEventsCommand<TIdentity> command)
        {
            var expectedVersion = command.ExpectedVersion;
            var events = command.Events
                .Select(x => new EventEnvelope(x, new EventMetadata(command.MessageId, x.GetType().AssemblyQualifiedName)))
                .ToList();

            if (expectedVersion.HasValue)
            {
                _eventStoreContext.Persist(command.Identity, events, expectedVersion.Value - 1);
            }
            else
            {
                _eventStoreContext.Persist(command.Identity, events);
            }
        }
    }
}
