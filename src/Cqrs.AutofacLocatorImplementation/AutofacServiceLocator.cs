using System;
using System.Diagnostics.Contracts;
using Autofac;
using Cqrs.Messaging.Configuration.Amqp;

namespace Cqrs.AutofacLocatorImplementation
{
    public class AutofacServiceLocator : IServiceRegistration, IServiceResolver
    {
        private readonly IContainer _container;

        public AutofacServiceLocator(IContainer container)
        {
            Contract.Requires(container != null);
            _container = container;
        }

        public TService Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }

        public void Register<TService, TImplementation>()
        {
            if (_container.IsRegistered<TService>())
            {
                return;
            }
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder
                .RegisterType<TImplementation>()
                .As<TService>()
                .SingleInstance();
            containerBuilder.Update(_container);
        }
        
        public void Register<TService>(Func<TService> componentRegistration)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(context => componentRegistration()).SingleInstance();
            containerBuilder.Update(_container);
        }
    }
}
