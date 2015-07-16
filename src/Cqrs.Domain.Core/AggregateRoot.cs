using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Domain.Core
{

    public abstract class AggregateRoot
    {
    }

    public abstract class AggregateRoot<TIdentity> 
        : AggregateRoot
        where TIdentity : Identity
    {
        public TIdentity Id { get; protected set; }

        private readonly Queue<IEvent> _uncommitedEvents;

        public AggregateRoot(TIdentity id)
        {
            Id = id;
            _uncommitedEvents = new Queue<IEvent>();
        }

        public IReadOnlyCollection<IEvent> GetUncommitedEvents()
        {
            return _uncommitedEvents.ToList();
        }

        public virtual void Raise(IEvent @event)
        {
            _uncommitedEvents.Enqueue(@event);
        }
    }
}
