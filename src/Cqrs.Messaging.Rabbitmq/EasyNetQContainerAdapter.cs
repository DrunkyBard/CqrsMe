using System.Diagnostics.Contracts;
using Cqrs.Messaging.Configuration.Amqp;
using EasyNetQ;

namespace Cqrs.Messaging.Rabbitmq
{
    internal sealed  class EasyNetQContainerAdapter : IContainer
    {
        private readonly ILocator _locator;

        public EasyNetQContainerAdapter(ILocator locator)
        {
            Contract.Requires(locator != null);
            _locator = locator;
        }

        public TService Resolve<TService>() where TService : class
        {
            return _locator.Resolve<TService>();
        }

        public IServiceRegister Register<TService>(System.Func<IServiceProvider, TService> serviceCreator) where TService : class
        {
            _locator.Register(() => serviceCreator(this));

            return this;
        }

        public IServiceRegister Register<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _locator.Register<TService, TImplementation>();

            return this;
        }
    }
}
