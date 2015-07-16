using System;

namespace Cqrs.Domain.ProcessManager.Exceptions
{
    public sealed class EventHandlerNotFoundException : Exception
    {
        public EventHandlerNotFoundException(string message) : base(message)
        {
        }
    }
}
