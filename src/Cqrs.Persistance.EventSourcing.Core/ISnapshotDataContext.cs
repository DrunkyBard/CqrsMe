using Cqrs.Domain.Core;
using Cqrs.Domain.EventSourcing;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public interface ISnapshotDataContext
    {
        SnapshotEnvelope<TSnapshot> Ask<TSnapshot, TIdentity>(TIdentity identity) 
            where TSnapshot : Snapshot
            where TIdentity : Identity;

        void Persist<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot;
    }
}
