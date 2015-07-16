using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cqrs.Messaging.Configuration.Amqp
{
    public sealed class GatewayConfigurationBuilder
    {
        private readonly IGatewayFactory _gatewayFactory;
        private List<Host> _hosts;
        private int _prefetchCount;
        private int _eventSubscriberThreadCount;
        private int _commandHandlerThreadCount;
        private GatewayConfiguration _gatewayConfiguration;
        private UserCrenedtial _userCrenedtial;
        private ILocator _serviceLocator;

        public GatewayConfigurationBuilder(IGatewayFactory gatewayFactory, ILocator locator) //TODO: Remove gateWay factory
        {
            Contract.Requires(gatewayFactory != null, "GatewayFactory should not be null");
            Contract.Requires(locator != null, "Locator should not be null");

            _gatewayFactory = gatewayFactory;
            _serviceLocator = locator;
             _hosts = new List<Host>();
            _userCrenedtial = new UserCrenedtial("guest", "guest");
            _eventSubscriberThreadCount = 1;
            _commandHandlerThreadCount = 1;
        }

        public GatewayConfigurationBuilder WithConfiguration(GatewayConfiguration gatewayConfiguration)
        {
            _gatewayConfiguration = gatewayConfiguration;

            return this;
        }

        public GatewayConfigurationBuilder WithHost(Host host)
        {
            return WithHosts(new List<Host> {host});
        }

        public GatewayConfigurationBuilder WithHosts(IReadOnlyCollection<Host> hosts)
        {
            _hosts = _hosts.Union(hosts, new HostEqualityComparer()).ToList();

            return this;
        }

        public GatewayConfigurationBuilder WithUserCredential(UserCrenedtial userCrenedtial)
        {
            _userCrenedtial = userCrenedtial;

            return this;
        }

        public GatewayConfigurationBuilder WithPrefetch(int count)
        {
            _prefetchCount = count;

            return this;
        }

        public GatewayConfigurationBuilder WithEventSubscriberThreads(int count)
        {
            _eventSubscriberThreadCount = count;

            return this;
        }

        public GatewayConfigurationBuilder WithCommandHandlerThreads(int count)
        {
            _commandHandlerThreadCount = count;

            return this;
        }

        public IGateway Build()
        {
            Invariant();
            FillBrokerConfiguration();
            return _gatewayFactory.Create(_gatewayConfiguration);
        }

        private void Invariant()
        {
            if (!_hosts.Any())
            {
                _hosts.Add(new Host("localhost", 5672));
            }
        }

        private void FillBrokerConfiguration()
        {
            if (_gatewayConfiguration == null)
            {
                _gatewayConfiguration = new GatewayConfiguration(_hosts, _userCrenedtial, _serviceLocator, _prefetchCount, _eventSubscriberThreadCount, _commandHandlerThreadCount);
            }
        }

        private class HostEqualityComparer : IEqualityComparer<Host>
        {
            public bool Equals(Host x, Host y)
            {
                return x.HostName.ToLower() == y.HostName.ToLower() && x.Port == y.Port;
            }

            public int GetHashCode(Host obj)
            {
                return obj.HostName.GetHashCode() ^ obj.Port;
            }
        }
    }
}
