using System;

namespace Cqrs.Domain.ProcessManager.Exceptions
{
    public sealed class EventHandlerAlreadyRegisteredException : Exception
    {
        public EventHandlerAlreadyRegisteredException(string message) : base(message)
        {
        }
    }
}
