using System.Collections.Generic;
using Cqrs.Messaging.Configuration.Amqp;

namespace Cqrs.Projection.Core
{
    public sealed class ProjectionGatewayConfiguration
    {
        public readonly ILocator Locator;
        public readonly MultiValueDictionary<string, Subscription> Subscriptions;

        internal ProjectionGatewayConfiguration(ILocator locator, MultiValueDictionary<string, Subscription> subscriptions)
        {
            Locator = locator;
            Subscriptions = subscriptions;
        }
    }
}
