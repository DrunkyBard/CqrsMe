using System;

namespace Cqrs.Projection.Core
{
    public sealed class Subscription
    {
        public readonly Type EventType;
        public readonly Action<object> EventHandler;

        public Subscription(Type eventType, Action<object> eventHandler)
        {
            EventType = eventType;
            EventHandler = eventHandler;
        }
    }
}
