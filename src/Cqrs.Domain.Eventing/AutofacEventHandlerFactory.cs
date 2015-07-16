using System.Collections.Generic;
using System.Linq;
using Autofac;
using Cqrs.Domain.Core;

namespace Cqrs.Domain.Eventing
{
    internal sealed class AutofacEventHandlerFactory : IEventHandlerFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacEventHandlerFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IReadOnlyCollection<IDomainEventHandler<TEvent>> Create<TEvent>() where TEvent : IEvent
        {
            var eventHandlers = _lifetimeScope.Resolve<IEnumerable<IDomainEventHandler<TEvent>>>().ToList();

            return eventHandlers;
        }
    }
}
