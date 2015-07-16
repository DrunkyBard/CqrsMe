using System.Collections.Generic;

namespace Cqrs.Messaging.Configuration.Amqp
{
    public class GatewayConfiguration
    {
        public readonly IReadOnlyCollection<Host> Hosts;
        public readonly UserCrenedtial UserCrenedtial;
        public readonly int PrefetchCount;
        public readonly int EventSubscriberThreadCount;
        public readonly int CommandHandlerThreadCount;
        public readonly ILocator Locator;

        public GatewayConfiguration(IReadOnlyCollection<Host> hosts, UserCrenedtial userCredential, ILocator locator, int prefetchCount, int eventSubscriberThreadCount, int commandHandlerThreadCount)
        {
            Hosts = hosts;
            UserCrenedtial = userCredential;
            Locator = locator;
            PrefetchCount = prefetchCount;
            EventSubscriberThreadCount = eventSubscriberThreadCount;
            CommandHandlerThreadCount = commandHandlerThreadCount;
        }
    }
}
