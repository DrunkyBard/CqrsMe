using System;
using Commands.Core;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.EventSourcing.Commands
{
    public class RestoreAggregateCommand<TIdentity, TSnapshot> : ICommand
        where TIdentity : Identity
        where TSnapshot : Snapshot
    {
        public readonly TIdentity Identity;

        public readonly Type SnapshotType;

        public RestoreAggregateCommand(TIdentity identity)
        {
            Identity = identity;
            SnapshotType = typeof (TSnapshot);
        }
    }
}
