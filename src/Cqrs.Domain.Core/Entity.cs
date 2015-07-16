namespace Cqrs.Domain.Core
{
    public abstract class Entity<TIdentity> where TIdentity : Identity
    {
        public readonly TIdentity Identity;

        public Entity(TIdentity identity)
        {
            Identity = identity;
        }
    }
}
