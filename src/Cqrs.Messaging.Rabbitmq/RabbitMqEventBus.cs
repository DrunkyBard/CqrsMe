using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cqrs.AntiCorruption;
using Cqrs.Common;
using Cqrs.Domain.Core;
using Cqrs.Domain.Eventing;
using Cqrs.Messaging.Core;
using EasyNetQ;
using EasyNetQ.NonGeneric;
using JetBrains.Annotations;
using IEventBus = Cqrs.Messaging.Core.IEventBus;

namespace Cqrs.Messaging.Rabbitmq
{
    [UsedImplicitly]
    internal sealed class RabbitMqEventBus : IEventBus
    {
        private readonly IAntiCorruption _antiCorruption;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IBus _bus;

        public RabbitMqEventBus(IAntiCorruption antiCorruption, IEventHandlerFactory eventHandlerFactory, IBus bus)
        {
            _antiCorruption = antiCorruption;
            _eventHandlerFactory = eventHandlerFactory;
            _bus = bus;
        }

        public IDisposable Subscribe<TEvent>() where TEvent : IEvent
        {
            var externalEventType = _antiCorruption.GetMappedEventType(typeof(TEvent));

            var subscription = _bus.SubscribeAsync(
                typeof(E<>).MakeGenericType(externalEventType),
                Assembly.GetEntryAssembly().GetName().Name,
                externalEvent => Task.Factory.StartNew(() =>
                {
                    var eventEnvelope = (E)externalEvent;
                    var mappedInternalEvent = _antiCorruption.MapExternalEvent<TEvent>(eventEnvelope.Body);
                    var @event = new Envelope<TEvent>(eventEnvelope.MessageId, mappedInternalEvent, eventEnvelope.IsRedelivered);
                    //TODO: Мб избавиться от Generic E<T>?

                    var eventHandlers = _eventHandlerFactory.Create<Envelope<TEvent>>();

                    foreach (var domainEventHandler in eventHandlers)
                    {
                        domainEventHandler.Handle(@event);
                    }
                }));

            return subscription;
        }

        public void Publish(IReadOnlyCollection<IEvent> events)
        {
            foreach (var @event in events)
            {
                Publish((dynamic)@event);
            }
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            var eventEnvelope = new E<TEvent>
            {
                Body = @event,
                IsRedelivered = false,
                MessageId = @event.ToGuid()
            };

            _bus.Publish(eventEnvelope.GetType(), eventEnvelope);
        }

        //public async void Publish(IReadOnlyCollection<IEvent> events)
        //{
        //    var externalEvents = _antiCorruption.MapInternalEvents(events);
            
        //    foreach (var externalEvent in externalEvents)
        //    {
        //        await _bus.Publish(externalEvent.GetType(), externalEvent);
        //    }
        //}
    }

    
}
