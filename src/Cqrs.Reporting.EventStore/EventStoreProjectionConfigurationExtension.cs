using System;
using System.Linq;
using System.Net;
using System.Text;
using Cqrs.Projection.Core;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cqrs.Projection.EventStore
{
    public static class EventStoreProjectionConfigurationExtension
    {
        public static void UseEventStore(this ProjectionGatewayConfiguration projectionGatewayConfiguration, ConnectionSettings connection, IPEndPoint httpEndPoint, IPEndPoint tcpEndPoint)
        {
            var eventStoreConnection = EventStoreConnection.Create(connection, tcpEndPoint);
            ProjectionManager.Configure(projectionGatewayConfiguration, httpEndPoint, connection.DefaultUserCredentials);
            Start(projectionGatewayConfiguration, eventStoreConnection);
        }
        
        public static void UseEventStore(this ProjectionGatewayConfiguration projectionGatewayConfiguration, ConnectionSettings connection, ClusterSettings clusterSettings, IPEndPoint tcpEndpoint)
        {
            var eventStoreConnection = EventStoreConnection.Create(connection, clusterSettings);
            ProjectionManager.Configure(projectionGatewayConfiguration, tcpEndpoint, connection.DefaultUserCredentials);
            Start(projectionGatewayConfiguration, eventStoreConnection);
        }

        private static void Start(ProjectionGatewayConfiguration projectionGatewayConfiguration, IEventStoreConnection eventStoreConnection)
            //TODO: Need restore point configuration
        {
            projectionGatewayConfiguration.Locator.Register(() => eventStoreConnection);
            eventStoreConnection.ConnectAsync().Wait();

            foreach (var subscription in projectionGatewayConfiguration.Subscriptions)
            {
                var eventHandlers = subscription.Value;
                eventStoreConnection.SubscribeToStreamFrom(
                    stream: subscription.Key + "View",
                    lastCheckpoint: StreamCheckpoint.StreamStart,
                    resolveLinkTos: true,
                    eventAppeared: (upSubscription, @event) =>
                    {
                        var eventType = Type.GetType(@event.Event.EventType);
                        var deserializedEvent = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(@event.Event.Data), eventType);
                        eventHandlers
                            .Single(x => x.EventType == eventType)
                            .EventHandler(deserializedEvent);
                    });
            }
        }

        private static class ProjectionManager
        {
            public static void Configure(ProjectionGatewayConfiguration projectionGatewayConfiguration, IPEndPoint httpEndPoint, UserCredentials credentials)
            {
                var projectionManager = new ProjectionsManager(new ConsoleLogger(), httpEndPoint, new TimeSpan(1, 0, 0, 0));
                var byCategoryProjectionStatus = ((JObject)JsonConvert.DeserializeObject(projectionManager.GetStatusAsync("$by_category", credentials).Result))["status"].ToString();
                var streamByCategoryProjectionStatus = ((JObject)JsonConvert.DeserializeObject(projectionManager.GetStatusAsync("$stream_by_category", credentials).Result))["status"].ToString();

                if (byCategoryProjectionStatus == "Stopped")
                {
                    projectionManager.EnableAsync("$by_category", credentials).Wait();
                }

                if (streamByCategoryProjectionStatus == "Stopped")
                {
                    projectionManager.EnableAsync("$stream_by_category", credentials).Wait();
                }
                const string projectionPattern = @"fromCategory('{0}')
                .foreachStream()
                .whenAny(function(state, event){{
                    linkTo('{1}', event);
                }})";
                
                foreach (var aggregateRootName in projectionGatewayConfiguration.Subscriptions.Keys)
                {
                    projectionManager.CreateContinuousAsync(
                        string.Format("{0}Projection", aggregateRootName),
                        string.Format(projectionPattern, aggregateRootName, aggregateRootName + "View"), credentials).Wait();
                }
            }
        }
    }
}
