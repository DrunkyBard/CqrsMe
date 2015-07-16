using System;
using System.Collections.Generic;
using Commands.Core;
using Cqrs.Domain.Core;
using Cqrs.Domain.EventSourcing;
using Cqrs.Messaging.Core;
using Cqrs.Messaging.Core.Commands;
using Cqrs.Persistance.EventSourcing.Core;
using Cqrs.Persistance.EventSourcing.Core.Commands;

namespace Cqrs.Application.CommandHandlers
{
    public abstract class IdempotentCommandHandler<TAggregate, TIdentity, TSnapshot, TCommand> : ICommandHandler<Envelope<TCommand>>
        where TCommand : ICommand<TIdentity>, IEventSourced
        where TIdentity : Identity
        where TSnapshot : Snapshot
        where TAggregate : EventSourcedAggregateRoot<TIdentity, TSnapshot>
    {
        private IReadOnlyCollection<Guid> _previouslyDispatchedMessages;
        private Guid _processingCommandId;
        protected readonly ICommandComposition CommandComposition;

        public IdempotentCommandHandler(ICommandComposition commandComposition)
        {
            CommandComposition = commandComposition;
        }

        public void Execute(Envelope<TCommand> command)
        {
            _processingCommandId = command.MessageId;
            var restoreAggregateCommand = new RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot>(command.Body.Identity, command.Body.Version, command.MessageId);
            var restoreAggregateResult = CommandComposition
                .StartWith<RestoreEventSourcedAggregateCommand<TIdentity, TSnapshot>, RestoreAggregateCommandResult<TAggregate>>(restoreAggregateCommand)
                .Run();
            _previouslyDispatchedMessages = restoreAggregateResult.PreviouslyDispatchedMessages;

            if (restoreAggregateResult.AlreadyDispatched)
            {
                var publishEvents = new PublishEventsCommand(restoreAggregateResult.NeedRedispatchEvents);
                CommandComposition
                    .StartWith(publishEvents)
                    .Run();
            }
            else
            {
                var aggregate = restoreAggregateResult.Aggregate;
                IdempotentExecute(aggregate, command.Body);
                Persist(aggregate);
            }
        }

        private void Persist(TAggregate aggregate)
        {
            var uncommitedEvents = aggregate.GetUncommitedEvents();
            var snapshot = aggregate.GetSnapshot();
            var originalVersion = aggregate.Version - uncommitedEvents.Count;
            var persistEvents = new PersistEventsCommand<TIdentity>(aggregate.Id, uncommitedEvents, _processingCommandId, originalVersion);
            var persistSnapshot = new PersistSnapshotCommand<TSnapshot>(snapshot, _previouslyDispatchedMessages, uncommitedEvents.Count);
            var publishEvents = new PublishEventsCommand(uncommitedEvents);

            CommandComposition
                .StartWith(persistEvents)
                .ContinueWith(persistSnapshot)
                .ContinueWith(publishEvents)
                .Run();
        }

        protected abstract void IdempotentExecute(TAggregate aggregate, TCommand command);
    }
}
