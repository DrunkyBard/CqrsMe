using System;
using System.Collections.Generic;
using Cqrs.Domain.Core;

namespace Cqrs.Persistance.EventSourcing.Core.Commands
{
    public sealed class PersistEventsCommand<TIdentity> : ICommand<TIdentity>
        where TIdentity : Identity
    {
        public TIdentity Identity { get; private set; }
        public readonly IReadOnlyCollection<IEvent> Events;
        public readonly Guid MessageId;
        public readonly int? ExpectedVersion;


        public PersistEventsCommand(TIdentity identity, IReadOnlyCollection<IEvent> events, Guid messageId, int? expectedVersion = null)
        {
            Identity = identity;
            Events = events;
            MessageId = messageId;
            ExpectedVersion = expectedVersion;
        }

    }
}
