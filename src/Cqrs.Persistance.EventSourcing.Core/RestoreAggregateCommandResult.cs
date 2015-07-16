using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Cqrs.Domain.Core;

namespace Cqrs.Persistance.EventSourcing.Core
{
    public sealed class RestoreAggregateCommandResult<TAggregate> where TAggregate : AggregateRoot
    {
        public readonly TAggregate Aggregate;
        public readonly IReadOnlyCollection<Guid> PreviouslyDispatchedMessages;
        public readonly bool AlreadyDispatched;
        public readonly IReadOnlyCollection<IEvent> NeedRedispatchEvents;

        public RestoreAggregateCommandResult(TAggregate aggregate, IReadOnlyCollection<Guid> previouslyDispatchedMessages, bool alreadyDispatched, IReadOnlyCollection<IEvent> needRedispatchEvents)
        {
            Contract.Requires(aggregate != null);
            Contract.Requires(previouslyDispatchedMessages != null);
            Contract.Requires(needRedispatchEvents != null);
            Contract.Requires(alreadyDispatched && needRedispatchEvents.Any() || !alreadyDispatched && !needRedispatchEvents.Any());
            Aggregate = aggregate;
            PreviouslyDispatchedMessages = previouslyDispatchedMessages;
            AlreadyDispatched = alreadyDispatched;
            NeedRedispatchEvents = needRedispatchEvents;
            
        }
    }
}
