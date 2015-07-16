using System.Collections.Generic;
using Cqrs.Domain.Core;

namespace Cqrs.Messaging.AntiCorruption
{
    public interface IExternalToInternalEventMapper
    {
        IEvent Map<TExternalEvent>(TExternalEvent externalEvent) where TExternalEvent : IEvent;

        IReadOnlyCollection<IEvent> Map<TExternalEvent>(IReadOnlyCollection<IEvent> externalEvents) where TExternalEvent : IEvent;
    }
}
