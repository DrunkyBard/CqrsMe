using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Cqrs.Common;
using Cqrs.Domain.Core;
using Cqrs.Persistance.EventSourcing.Core;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cqrs.Persistance.EventSourcing.EventStore
{
    [UsedImplicitly]
    internal sealed class EventStoreDataContext : IEventStoreContext
    {
        private readonly IEventStoreConnection _eventStoreConnection;

        public EventStoreDataContext()
        {
            var eventStoreConnectionSettings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                .UseConsoleLogger()
                .Build();
            _eventStoreConnection = EventStoreConnection.Create(eventStoreConnectionSettings, new IPEndPoint(IPAddress.Loopback, 1113));
            _eventStoreConnection.ConnectAsync().Wait();
        }

        public IReadOnlyCollection<EventEnvelope> AskEventsFor<TIdentity>(TIdentity aggregateIdentity) 
            where TIdentity : Identity
        {
            var events = AskEventsFor(aggregateIdentity, StreamPosition.Start, int.MaxValue);

            return events;
        }

        public IReadOnlyCollection<EventEnvelope> AskEventsFor<TIdentity>(TIdentity aggregateIdentity, int versionFrom) 
            where TIdentity : Identity
        {
            var events = AskEventsFor(aggregateIdentity, versionFrom, int.MaxValue);

            return events;
        }

        public IReadOnlyCollection<EventEnvelope> AskEventsFor<TIdentity>(TIdentity aggregateIdentity, int versionFrom, int versionTo) 
            where TIdentity : Identity
        {
            var resolvedEvents = _eventStoreConnection
                .ReadStreamEventsForwardAsync(aggregateIdentity.ToStringRepresentation(), versionFrom, versionTo, false)
                .Result
                .Normilize();
            
            return resolvedEvents;
        }

        public void Persist<TIdentity>(TIdentity identity, IReadOnlyCollection<EventEnvelope> events) where TIdentity : Identity
        {
            Persist(identity, events, ExpectedVersion.EmptyStream);
        }

        public void Persist<TIdentity>(TIdentity identity, IReadOnlyCollection<EventEnvelope> events, int expectedVersion) where TIdentity : Identity
        {
            var eventData = events
                .Select(envelope =>
                {
                    var eventJsonBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(envelope.Event));
                    var metadataJsonBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(envelope.Metadata));
                    var eventType = envelope.Event.GetType().AssemblyQualifiedName;
                    var messageIdentity = new { envelope.Metadata.DispatchedMessageId, envelope.Event }.ToGuid();
                    
                    return new EventData(messageIdentity, eventType, true, eventJsonBinary, metadataJsonBinary);
                })
                .ToList();
            _eventStoreConnection.AppendToStreamAsync(identity.ToStringRepresentation(), expectedVersion, eventData).Wait();
            //var transaction = _eventStoreConnection.StartTransactionAsync(identity.ToStringRepresentation(), expectedVersion).Result;
            //transaction.WriteAsync(eventData).Wait();
            //transaction.CommitAsync().Wait();
        }
    }
}
