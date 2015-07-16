using Cqrs.Domain.Core;

namespace Cqrs.Domain.EventSourcing
{
    public abstract class Snapshot : IEventSourced
    {
        public readonly Identity Identity;
        public int Version { get; private set; }

        public Snapshot(int version, Identity identity)
        {
            Identity = identity;
            Version = version;
        }
    }
}
