using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Commands.Core;
using Cqrs.Domain.Core;
using Cqrs.Domain.EventSourcing;
using Cqrs.Persistance.EventSourcing.Core.Commands;
using JetBrains.Annotations;

namespace Cqrs.Persistance.EventSourcing.Core.CommandHandlers
{
    [UsedImplicitly]
    public sealed class RestoreEventSourcedAggregateCommandHandler<TIdentity, TAggregate, TSnapshot>
        : ICommandHandler<RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot>, RestoreAggregateCommandResult<TAggregate>> 
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

        public RestoreAggregateCommandResult<TAggregate> Execute(RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot> command)
        {
            var snapshotEnvelope = _snapshotDataContext.Ask<TSnapshot, TIdentity>(command.Identity);
            IReadOnlyCollection<EventEnvelope> eventEnvelopes;
            TAggregate aggregate;
            var previouslyDispatchedMessages = new List<Guid>();
            int currentVersion;

            if (snapshotEnvelope != null)
            {
                eventEnvelopes = _eventStoreContext.AskEventsFor(command.Identity, snapshotEnvelope.Snapshot.Version);
                previouslyDispatchedMessages.AddRange(snapshotEnvelope.Metadata.DispatchedMessageIds);
                currentVersion = snapshotEnvelope.Snapshot.Version + eventEnvelopes.Count;
                aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), command.Identity, snapshotEnvelope.Snapshot, eventEnvelopes);
            }
            else
            {
                eventEnvelopes = _eventStoreContext.AskEventsFor(command.Identity);
                var events = eventEnvelopes.Select(x => x.Event).ToList();
                currentVersion = eventEnvelopes.Count;
                aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), command.Identity, events);
            }

            _dispatchedMessages.AddRange(eventEnvelopes.Select(x => x.Metadata.DispatchedMessageId));
            previouslyDispatchedMessages = previouslyDispatchedMessages.Union(_dispatchedMessages, new GuidComparer()).ToList();

            if (previouslyDispatchedMessages.Contains(command.IncomingMessageId))
            {
                var needPublishedEvents = eventEnvelopes
                    .Where(x => x.Metadata.DispatchedMessageId == command.IncomingMessageId)
                    .Select(x => x.Event)
                    .ToList();

                return new RestoreAggregateCommandResult<TAggregate>(aggregate, _dispatchedMessages, true, needPublishedEvents);
            }

            var expectedVersion = command.Version;

            if (currentVersion > expectedVersion || aggregate.Version != expectedVersion)
            {
                throw new InvalidOperationException();
            }

            _dispatchedMessages.Add(command.IncomingMessageId);

            return new RestoreAggregateCommandResult<TAggregate>(aggregate, _dispatchedMessages, false, Enumerable.Empty<IEvent>().ToList());
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
