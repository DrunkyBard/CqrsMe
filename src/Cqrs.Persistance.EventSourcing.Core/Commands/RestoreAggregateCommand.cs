using System;
using Commands.Core;
using Cqrs.Domain.Core;
using Cqrs.Domain.EventSourcing;

namespace Cqrs.Persistance.EventSourcing.Core.Commands
{
    public class RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot> : ICommand
        where TIdentity : Identity
        where TSnapshot : Snapshot
    {
        public readonly TIdentity Identity;
        public readonly int Version;
        public readonly Guid IncomingMessageId;
        public readonly Type SnapshotType;

        public RestoreEventSourcedAggregateCommand(TIdentity identity, int version, Guid incomingMessageId)
        {
            Identity = identity;
            Version = version;
            IncomingMessageId = incomingMessageId;
            SnapshotType = typeof (TSnapshot);
        }
    }
}
