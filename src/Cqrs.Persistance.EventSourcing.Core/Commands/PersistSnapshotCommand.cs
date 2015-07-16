using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Commands.Core;
using Cqrs.Domain.EventSourcing;

namespace Cqrs.Persistance.EventSourcing.Core.Commands
{
    public sealed class PersistSnapshotCommand<TSnapshot> : ICommand
        where TSnapshot : Snapshot
    {
        public readonly TSnapshot Snapshot;
        public readonly int ProducedEventCount;
        public readonly IReadOnlyCollection<Guid> PreviouslyDispatchedCommandIds;

        public PersistSnapshotCommand(TSnapshot snapshot, IReadOnlyCollection<Guid> previouslyDispatchedCommandIds, int producedEventCount)
        {
            Contract.Requires(snapshot != null);
            Contract.Requires(previouslyDispatchedCommandIds != null && previouslyDispatchedCommandIds.Any());

            Snapshot = snapshot;
            ProducedEventCount = producedEventCount;
            PreviouslyDispatchedCommandIds = previouslyDispatchedCommandIds.Distinct().ToList();
        }
    }
}
