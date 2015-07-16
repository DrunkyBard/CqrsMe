using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Commands.Core;
using Cqrs.Domain.Core;
using Cqrs.Domain.EventSourcing.Commands;
using Cqrs.Persistance.EventSourcing.Core;
using JetBrains.Annotations;

namespace Cqrs.Domain.EventSourcing.CommandHandlers
{
    [UsedImplicitly]
    internal sealed class RestoreEventSourcedAggregateCommandHandler<TIdentity, TAggregate, TSnapshot> 
        : ICommandHandler<RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot>, TAggregate> 
        where TIdentity : Identity
        where TAggregate : EventSourcedAggregateRoot<TIdentity, TSnapshot>
        where TSnapshot : Snapshot
    {
        private readonly IEventStoreContext _eventStoreContext;
        private readonly ISnapshotDataContext _snapshotDataContext;
        private readonly List<Guid> _dispatchedMessages; 

        public RestoreEventSourcedAggregateCommandHandler(IEventStoreContext eventStoreContext, ISnapshotDataContext snapshotDataContext)
        {
            Contract.Requires(eventStoreContext != null, "EventStoreContext should not be null");
            Contract.Requires(snapshotDataContext != null, "SnapshotDataContext should not be null");
            _eventStoreContext = eventStoreContext;
            _snapshotDataContext = snapshotDataContext;
            _dispatchedMessages = new List<Guid>();
        }

        public TAggregate Execute(RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot> command)
        {
            var snapshotEnvelope = _snapshotDataContext.Ask<TSnapshot, TIdentity>(command.Identity);
            IReadOnlyCollection<EventEnvelope> eventEnvelopes;
            TAggregate aggregate;
            var previouslyDispatchedMessages = new List<Guid>();
            int currentVersion;
            var expectedVersion = command.Version;

            if (snapshotEnvelope != null)
            {
                eventEnvelopes = _eventStoreContext.AskEventsFor(command.Identity, snapshotEnvelope.Snapshot.Version);
                previouslyDispatchedMessages.AddRange(snapshotEnvelope.Metadata.DispatchedMessageIds);
                currentVersion = snapshotEnvelope.Snapshot.Version + eventEnvelopes.Count;
                aggregate = (TAggregate)Activator.CreateInstance(typeof (TAggregate), snapshotEnvelope.Snapshot, eventEnvelopes);
            }
            else
            {
                eventEnvelopes = _eventStoreContext.AskEventsFor(command.Identity);
                currentVersion = eventEnvelopes.Count;
                aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), eventEnvelopes);
            }

            _dispatchedMessages.AddRange(eventEnvelopes.Select(x => x.Metadata.DispatchedMessageId));
            previouslyDispatchedMessages = previouslyDispatchedMessages.Union(_dispatchedMessages, new GuidComparer()).ToList();

            if (previouslyDispatchedMessages.Contains(command.IncomingMessageId))
            {
                var needPublisherEvents = eventEnvelopes.Where(x => x.Metadata.DispatchedMessageId == command.IncomingMessageId).ToList();

                return null; // TODO: Interrupt command processing
            }

            if (currentVersion > expectedVersion)
            {
                throw new InvalidOperationException();
            }

            _dispatchedMessages.Add(command.IncomingMessageId);

            return aggregate;
        }

        private class GuidComparer : IEqualityComparer<Guid>
        {
            public bool Equals(Guid x, Guid y)
            {
                return x == y;
            }

            public int GetHashCode(Guid obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
