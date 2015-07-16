using System;
using System.Collections.Generic;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public sealed class SnapshotMetadata
    {
        public readonly IReadOnlyCollection<Guid> DispatchedMessageIds;

        public SnapshotMetadata(IReadOnlyCollection<Guid> dispatchedMessageIds)
        {
            DispatchedMessageIds = dispatchedMessageIds;
        }
    }
}
