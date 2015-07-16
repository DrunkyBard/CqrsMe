using Cqrs.Domain.EventSourcing;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public sealed class SnapshotEnvelope<TSnapshot> : Snapshot
        where TSnapshot : Snapshot
    {
        public readonly TSnapshot Snapshot;
        public readonly SnapshotMetadata Metadata;

        public SnapshotEnvelope(TSnapshot snapshot, SnapshotMetadata metadata) : base(snapshot.Version, snapshot.Identity)
        {
            Snapshot = snapshot;
            Metadata = metadata;
        }
    }
}
