using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.EventSourcing
{
    public abstract class EventSourcedAggregateRoot<TIdentity, TSnapshot>
        : AggregateRoot<TIdentity>, IEventSourced, IOriginator<TSnapshot>
        where TIdentity : Identity 
        where TSnapshot : Snapshot
    {

        public int Version { get; private set; }

        public EventSourcedAggregateRoot(TIdentity identity) : this(identity, Enumerable.Empty<IEvent>().ToList())
        {
        }

        public EventSourcedAggregateRoot(TIdentity identity, IReadOnlyCollection<IEvent> events) : base(identity)
        {
            Contract.Requires(events != null, "Events should not be null");
            Version = events.Count;
        }

        public EventSourcedAggregateRoot(TIdentity identity, TSnapshot snapshot, IReadOnlyCollection<IEvent> events) : base(identity)
        {
            Contract.Requires(snapshot != null, "Snapshot should not be null");
            Contract.Requires(events != null, "Events should not be null");
            Version = snapshot.Version + events.Count;
        }

        public sealed override void Raise(IEvent @event)
        {
            Version++;
            base.Raise(@event);
        }

        public abstract TSnapshot GetSnapshot();
    }
}
