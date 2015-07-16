using System.Collections.Generic;
using Cqrs.Domain.Core;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public interface IEventStoreContext
    {
        IReadOnlyCollection<EventEnvelope> AskEventsFor<TIdentity>(TIdentity aggregateIdentity)
            where TIdentity : Identity;

        IReadOnlyCollection<EventEnvelope> AskEventsFor<TIdentity>(TIdentity aggregateIdentity, int versionFrom) 
            where TIdentity : Identity;

        IReadOnlyCollection<EventEnvelope> AskEventsFor<TIdentity>(TIdentity aggregateIdentity, int versionFrom, int versionTo) 
            where TIdentity : Identity;

        void Persist<TIdentity>(TIdentity identity, IReadOnlyCollection<EventEnvelope> @events) where TIdentity : Identity;

        void Persist<TIdentity>(TIdentity identity, IReadOnlyCollection<EventEnvelope> @events, int expectedVersion) where TIdentity : Identity;
    }
}
