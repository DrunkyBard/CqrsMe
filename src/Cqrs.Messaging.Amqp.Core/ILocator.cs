using System;

namespace Cqrs.Messaging.Configuration.Amqp
{
    public interface ILocator
    {
        TService Resolve<TService>();

        void Register<TService, TImplementation>();

        void Register<TService>(Func<TService> componentRegistration);
    }
}
