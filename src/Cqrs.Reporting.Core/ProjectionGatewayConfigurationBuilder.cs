using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Cqrs.Common;
using Cqrs.Domain.Eventing;
using Cqrs.Messaging.Configuration.Amqp;

namespace Cqrs.Projection.Core
{
    public sealed class ProjectionGatewayConfigurationBuilder
    {
        private readonly ILocator _locator;
        internal readonly MultiValueDictionary<string, Subscription> Subscriptions;

        private ProjectionGatewayConfigurationBuilder(ILocator locator)
        {
            _locator = locator;
            var stringEqComparer = EqualityComparerFactory.Create<string>(
                (x, y) => x.ToLower() == y.ToLower(), 
                s => s.GetHashCode());
            Subscriptions = new MultiValueDictionary<string, Subscription>(stringEqComparer);
        }

        public static ProjectionGatewayConfigurationBuilder Create(ILocator locator) //TODO: Change project dependencies: Move gateway things from Amqp module
        {
            Contract.Requires(locator != null, "Locator should not be null");

            return new ProjectionGatewayConfigurationBuilder(locator);
        }

        public AggregateSubscriberConfigurationBuilder For(string aggregateRoot)
        {
            Contract.Requires(!string.IsNullOrEmpty(aggregateRoot), "AggregateRoot should not be null or empty");
            
            return new AggregateSubscriberConfigurationBuilder(this, _locator.Resolve<IEventHandlerFactory>(), aggregateRoot);
        }

        public ProjectionGatewayConfiguration Build()
        {
            return new ProjectionGatewayConfiguration(_locator, Subscriptions);
        }
    }
}
