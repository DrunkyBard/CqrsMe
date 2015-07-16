using System.Linq;
using Cqrs.Messaging.Configuration.Amqp;

namespace Cqrs.Messaging.Rabbitmq
{
    internal static class BrokerConfigurationExtensions
    {
        internal static string ToConnectionString(this GatewayConfiguration gatewayConfiguration)
        {
            const string hostPattern = "{0}:{1}";
            const string connectionStringPattern = "host={0};username={1};password={2};prefetchcount={3}";

            var hostStringArray = gatewayConfiguration.Hosts
                .Select(x => string.Format(hostPattern, x.HostName, x.Port))
                .ToArray();
            var hosts = string.Join(",", hostStringArray);

            return string.Format(connectionStringPattern, hosts, gatewayConfiguration.UserCrenedtial.UserName,
                gatewayConfiguration.UserCrenedtial.Password, gatewayConfiguration.PrefetchCount);
        }
    }
}
