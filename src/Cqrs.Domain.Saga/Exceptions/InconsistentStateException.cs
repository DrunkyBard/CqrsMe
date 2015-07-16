using System;

namespace Cqrs.Domain.ProcessManager.Exceptions
{
    public sealed class InconsistentStateException : Exception
    {
        public InconsistentStateException(string message) : base(message)
        {
        }
    }
}
