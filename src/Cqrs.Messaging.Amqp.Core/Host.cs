using System.Diagnostics.Contracts;

namespace Cqrs.Messaging.Configuration.Amqp
{
    public sealed class Host
    {
        public readonly string HostName;
        public readonly int Port;

        public Host(string hostName, int port)
        {
            Contract.Requires(!string.IsNullOrEmpty(hostName));
            Contract.Requires(port > 0);
            HostName = hostName;
            Port = port;
        }
    }
}
