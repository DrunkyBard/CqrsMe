using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cqrs.Domain.Core;
using Cqrs.Persistance.EventSourcing.Core;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Cqrs.Persistance.EventSourcing.EventStore
{
    internal static class EventStoreExtensions
    {
        internal static IReadOnlyCollection<EventEnvelope> Normilize(this StreamEventsSlice streamEventsSlice)
        {
            return streamEventsSlice.Events
                .Select(Map)
                .ToList();
        }

        private static EventEnvelope Map(ResolvedEvent resolvedEvent)
        {
            var bytesAsString = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            var a = JsonConvert.DeserializeObject(bytesAsString) as IEvent;
            var jsonMetadata = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
            var metadata = JsonConvert.DeserializeObject<EventMetadata>(jsonMetadata);
            var typeName = Type.GetType(metadata.Type);
            var @event = (IEvent)JsonConvert.DeserializeObject(bytesAsString, typeName);
            var envelope = new EventEnvelope(@event, metadata);

            return envelope;
        }

        internal static string ToStringRepresentation(this Identity identity)
        {
            //return string.Format("{0}-{1}-{2}", "AggregateRoot", identity.GetTag(), identity.GetId());
            return string.Format("{0}-{1}", identity.GetTag(), identity.GetId());
        }
    }
}
