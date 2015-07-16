namespace Cqrs.Domain.Core
{
    public interface IEvent
    {
    }

    public abstract class Event<TAggregateIdentity> : IEvent 
        where TAggregateIdentity : Identity
    {
        public readonly TAggregateIdentity Identity;

        public Event(TAggregateIdentity identity)
        {
            Identity = identity;
        }
    }
}
