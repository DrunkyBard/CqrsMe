using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Commands.Core;
using Cqrs.Application.CommandHandlers;
using Cqrs.AutofacLocatorImplementation;
using Cqrs.Domain.Core;
using Cqrs.Domain.Eventing;
using Cqrs.Domain.EventSourcing;
using Cqrs.Messaging.Configuration.Amqp;
using Cqrs.Messaging.Core;
using Cqrs.Messaging.Core.Commands;
using Cqrs.Persistance.EventSourcing.Core;
using Cqrs.Persistance.EventSourcing.Core.Commands;
using Cqrs.Projection.Core;
using Cqrs.Projection.EventStore;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestMessaging
{
    class SnapshotDataContext : ISnapshotDataContext
    {
        private readonly ConcurrentDictionary<string, object> _snapshots = new ConcurrentDictionary<string, object>();

        public SnapshotEnvelope<TSnapshot> Ask<TSnapshot, TIdentity>(TIdentity identity)
            where TSnapshot : Snapshot
            where TIdentity : Identity
        {
            var stringIdentity = string.Format("{0}-{1}", identity.GetTag(), identity.GetId());
            object snapshot;
            if (_snapshots.TryGetValue(stringIdentity, out snapshot))
            {
                return (SnapshotEnvelope<TSnapshot>)snapshot;
            }

            return null;
        }

        public void Persist<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
        {
            var snapshotIdentity = string.Format("{0}-{1}", snapshot.Identity.GetTag(), snapshot.Identity.GetId());
            _snapshots.AddOrUpdate(snapshotIdentity, snapshot, (s, o) => o);
        }
    }

    public class AnotherAggregateCreated : IEvent
    {
        public AggregateIdentity Identity { get; private set; }

        public AnotherAggregateCreated(AggregateIdentity identity)
        {
            Identity = identity;
        }
    }

    class MyClass11 : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = reader.ReadAsString();
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

    class EnvelopeDto<TMessage> where TMessage : ICommand
    {
        public TMessage Body { get; set; }

        public Guid MessageId { get; set; }

        public bool IsRedelivered { get; set; }

        public static implicit operator Envelope<TMessage>(EnvelopeDto<TMessage> d)
        {
            return new Envelope<TMessage>(d.MessageId, d.Body, d.IsRedelivered);
        }
    }

    class Converter : ITypeConverter<EnvelopeDto<C>, Envelope<C>>
    {
        public Envelope<C> Convert(ResolutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {

        private static Guid MungeTwoGuids(Guid guid1, Guid guid2)
        {
            const int BYTECOUNT = 16;
            byte[] destByte = new byte[BYTECOUNT];
            byte[] guid1Byte = guid1.ToByteArray();
            byte[] guid2Byte = guid2.ToByteArray();

            for (int i = 0; i < BYTECOUNT; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }



        static void Main()
        {
            //var cnt = ConnectionSettings
            //    .Create()
            //    .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
            //    .Build();
            //var eventStoreConnection = EventStoreConnection.Create(cnt, new IPEndPoint(IPAddress.Loopback, 1113));
            //eventStoreConnection.ConnectAsync().Wait();

            //eventStoreConnection.SubscribeToStreamFrom(
            //        stream: "Aggregate"+"View",
            //        lastCheckpoint: StreamCheckpoint.StreamStart,
            //        resolveLinkTos: true,
            //        eventAppeared: (upSubscription, @event) =>
            //        {
            //            var eventType = Type.GetType(@event.Event.EventType);
            //        }, userCredentials: new UserCredentials("admin", "changeit"));
            //Console.ReadKey();
            Mapper.CreateMap<AggregateCreated, AnotherAggregateCreated>()
                .ConstructUsing(created => new AnotherAggregateCreated(created.Identity))
                .ForMember(x => x.Identity, x => x.Ignore());
            Mapper.CreateMap<AggregateNameCreated, AggregateNameCreated>()
                .ConstructUsing(created => new AggregateNameCreated(created.Identity, created.Name))
                .ForMember(x => x.Identity, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore());
            Mapper.CreateMap<AnotherAggregateCreated, AggregateCreated>()
                .ConstructUsing(created => new AggregateCreated(created.Identity))
                .ForMember(x => x.Identity, x => x.Ignore());
            //using (MemoryStream tempStream = new MemoryStream())
            //{
            //    BinaryFormatter binFormatter = new BinaryFormatter(null,
            //        new StreamingContext(StreamingContextStates.Clone));

            //    binFormatter.Serialize(tempStream, envel);
            //    tempStream.Seek(0, SeekOrigin.Begin);

            //    var clone = binFormatter.Deserialize(tempStream);
            //}
            //var envelDto = new EnvelopeDto<C>
            //{
            //    Body = envel.Body,
            //    IsRedelivered = false,
            //    MessageId = envel.MessageId
            //};
            //var h = JsonConvert.SerializeObject(envelDto);
            //var t = (EnvelopeDto<C>)JsonConvert.DeserializeObject(h, typeof(EnvelopeDto<C>));
            //Envelope<C> qq = t;
            //var envAc = Activator.CreateInstance(typeof (Envelope<C>), t.MessageId, t.Body, false);
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<Cqrs.Messaging.Rabbitmq.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.Domain.Eventing.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Commands.Core.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.ObjectMapping.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.AntiCorruption.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.Messaging.Core.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.Persistance.EventSourcing.Core.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.Persistance.EventSourcing.EventStore.TypeRegistrationModule>();
            containerBuilder.RegisterModule<Cqrs.AntiCorruption.TypeRegistrationModule>();
            containerBuilder.RegisterType<SnapshotDataContext>()
                .As<ISnapshotDataContext>()
                .SingleInstance();
            containerBuilder.RegisterType<CreateAggregateCommandHandler>()
                .AsImplementedInterfaces().InstancePerDependency();
            containerBuilder.RegisterType<ChangeAggregateNameCommandHandler>()
                .AsImplementedInterfaces().InstancePerDependency();
            containerBuilder.RegisterType<AggregateCreatedEventHandler>()
                .AsImplementedInterfaces().InstancePerDependency();
            containerBuilder.RegisterType<AggregateNameEventHandler>()
                .AsImplementedInterfaces().InstancePerDependency();
            containerBuilder.RegisterType<AggregateProjection>()
                .AsImplementedInterfaces().InstancePerDependency();
            //var j = typeof(RestoreEventSourcedAggregateCommandHandler<,,>)
            //    .GetInterfaces().ToList();
            //containerBuilder.RegisterGeneric(typeof(RestoreEventSourcedAggregateCommandHandler<,,>))
            //    .As(typeof(ICommandHandler<,>))
            //    .AsSelf()
            //    .InstancePerDependency();

            var container = containerBuilder.Build();

            var ch = container.Resolve<ICommandHandler<RestoreEventSourcedAggregateCommand<AggregateIdentity, AggregateSnapshot>, RestoreAggregateCommandResult<Aggregate>>>();
            var autofacAdapter = new AutofacLocator(container);
            var gatewayFactory = container.Resolve<IGatewayFactory>();
            var gateWayBuilder = new GatewayConfigurationBuilder(gatewayFactory, autofacAdapter);
            var gateWay = gateWayBuilder
                .WithEventSubscriberThreads(10)
                .WithCommandHandlerThreads(10)
                .WithPrefetch(50)
                .Build();
            gateWay
                .ForCommands()
                .Handle<C>()
                .Handle<Ch>();

            gateWay.ForEvents()
                .Subscribe<AnotherAggregateCreated>()
                .Subscribe<AggregateNameCreated>();

            ProjectionGatewayConfigurationBuilder
                .Create(autofacAdapter)
                .For("Aggregate")
                .SubscribeTo<AggregateCreated>()
                .SubscribeTo<AggregateNameCreated>()
                .Build()
                .UseEventStore(
                    ConnectionSettings.Create()
                        .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                        .KeepRetrying()
                        .UseConsoleLogger()
                        .Build(),
                    new IPEndPoint(IPAddress.Loopback, 2113),
                    new IPEndPoint(IPAddress.Loopback, 1113)
                );

//            var userCredentials = new UserCredentials("admin", "changeit");
//            var projectionManager = new ProjectionsManager(new ConsoleLogger(), new IPEndPoint(IPAddress.Loopback, 2113), new TimeSpan(1, 0, 0));
//            var byCategoryProjectionStatus = ((JObject)JsonConvert.DeserializeObject(projectionManager.GetStatusAsync("$by_category", userCredentials).Result))["status"].ToString();
//            var streamByCategoryProjectionStatus = ((JObject)JsonConvert.DeserializeObject(projectionManager.GetStatusAsync("$stream_by_category", userCredentials).Result))["status"].ToString();

//            if (byCategoryProjectionStatus == "Stopped")
//            {
//                projectionManager.EnableAsync("$by_category", userCredentials).Wait();
//            }

//            if (streamByCategoryProjectionStatus == "Stopped")
//            {
//                projectionManager.EnableAsync("$stream_by_category", userCredentials).Wait();
//            }

//            const string projectionQuery = @"fromCategory('AggregateRoot')
//                                    .foreachStream()
//                                    .whenAny(function(state, event){
//                                         linkTo('AggregateBoundedContext', event);
//                                    })";
//            string aggregateCreatedProjectionQuery = @"fromCategory('AggregateRoot')
//                                    .foreachStream()
//                                    .when({'" + typeof(AggregateCreated).FullName + "':" +
//                                                     "function(state, event){linkTo('AggregateCreatedView', event);}" +
//                                                     "});";
//            string aggregateNameCreatedProjectionQuery = @"
//options({
//processingLag: 500
//});
//fromCategory('AggregateRoot')
//                                    .foreachStream()
//                                    .when({'" + typeof(AggregateNameCreated).FullName + "':" +
//                                                     "function(state, event){linkTo('AggregateNameCreatedView', event);}" +
//                                                     "});";
//            projectionManager.CreateContinuousAsync("AggregateProjection", projectionQuery, userCredentials).Wait();
            //projectionManager.CreateContinuousAsync("AggregateCreatedProjection", aggregateCreatedProjectionQuery, userCredentials).Wait();
            //projectionManager.CreateContinuousAsync("AggregateNameCreatedProjection", aggregateNameCreatedProjectionQuery, userCredentials).Wait();


            Console.ReadKey();
            var bus = container.Resolve<ICommandBus>();
            var aggregateId = Guid.NewGuid();
            var anotherAggregateId = Guid.NewGuid();
            var createAggregateCommand = new C(new AggregateIdentity(aggregateId), "NewAggregate");
            bus.Dispatch(createAggregateCommand);
            //bus.Dispatch(new C(new AggregateIdentity(Guid.NewGuid()), "AAA"));
            Console.ReadKey();

            var changeAggregateNameCommand = new Ch(2, new AggregateIdentity(aggregateId), "ChangedName");
            bus.Dispatch(changeAggregateNameCommand);

            Console.ReadKey();
            var changeAggregateNameCommand1 = new Ch(3, new AggregateIdentity(aggregateId), "ChangedName1");
            bus.Dispatch(changeAggregateNameCommand1);
            Console.ReadKey();

            var createAggregateCommand1 = new C(new AggregateIdentity(anotherAggregateId), "NewAggregate");
            bus.Dispatch(createAggregateCommand1);
            Console.ReadKey();
            var changeAggregateNameCommand13 = new Ch(2, new AggregateIdentity(anotherAggregateId), "ChangedName");
            bus.Dispatch(changeAggregateNameCommand13);

            Console.ReadKey();
            var changeAggregateNameCommand12 = new Ch(3, new AggregateIdentity(anotherAggregateId), "ChangedName1");
            bus.Dispatch(changeAggregateNameCommand12);
            
            Console.ReadKey();
        }
    }

    public class AggregateNameEventHandler : IDomainEventHandler<Envelope<AggregateNameCreated>>
    {
        public void Handle(Envelope<AggregateNameCreated> @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("Aggregate name created with {0} {1}", @event.Body.Identity.GetId(), @event.Body.Name));
        }
    }

    public class AggregateProjection
        : IDomainEventHandler<AggregateCreated>, IDomainEventHandler<AggregateNameCreated>
    {
        public void Handle(AggregateCreated @event)
        {
            Console.WriteLine("AggregateCreated Projection catched");
        }

        public void Handle(AggregateNameCreated @event)
        {
            Console.WriteLine("AggregateNameCreated Projection catched");
        }
    }

    public class AggregateCreatedEventHandler : IDomainEventHandler<Envelope<AnotherAggregateCreated>>
    {
        public void Handle(Envelope<AnotherAggregateCreated> @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("Aggregate created with {0}", @event.Body.Identity.GetId()));
        }
    }

    public class C : ICommand<AggregateIdentity>
    {
        public readonly string Name;

        public AggregateIdentity Identity { get; private set; }

        public C(AggregateIdentity identity, string name)
        {
            Name = name;
            Identity = identity;
        }
    }

    public class CreateAggregateCommandHandler : ICommandHandler<Envelope<C>>
    {
        private readonly ICommandComposition _commandComposition;

        public CreateAggregateCommandHandler(ICommandComposition commandComposition)
        {
            _commandComposition = commandComposition;
        }

        public void Execute(Envelope<C> command)
        {
            var messsageId = Guid.NewGuid();
            var newAggregate = new Aggregate(command.Body.Identity);
            newAggregate.SetName(command.Body.Name);

            var persistEvents = new PersistEventsCommand<AggregateIdentity>(newAggregate.Id, newAggregate.GetUncommitedEvents(), command.MessageId);
            var persistSnapshot = new PersistSnapshotCommand<AggregateSnapshot>(newAggregate.GetSnapshot(), Enumerable.Empty<Guid>().ToList(), 1);
            var publishEvents = new PublishEventsCommand(newAggregate.GetUncommitedEvents());

            _commandComposition
                .StartWith(persistEvents)
                .ContinueWith(persistSnapshot)
                .ContinueWith(publishEvents)
                .Run();
        }
    }

    public abstract class IdempotentAggregateCommandHandler<TCommand> :
        IdempotentCommandHandler<Aggregate, AggregateIdentity, AggregateSnapshot, TCommand>
        where TCommand : ICommand<AggregateIdentity>, IEventSourced
    {
        protected IdempotentAggregateCommandHandler(ICommandComposition commandComposition)
            : base(commandComposition)
        {
        }
    }

    public class ChangeAggregateNameCommandHandler : IdempotentAggregateCommandHandler<Ch>
    {
        public ChangeAggregateNameCommandHandler(ICommandComposition commandComposition)
            : base(commandComposition)
        { }

        protected override void IdempotentExecute(Aggregate aggregate, Ch command)
        {
            aggregate.SetName(command.NewName);
        }
    }

    public class Ch : ICommand<AggregateIdentity>, IEventSourced
    {
        public readonly string NewName;
        public AggregateIdentity Identity { get; private set; }
        public int Version { get; private set; }

        public Ch(int version, AggregateIdentity identity, string newName)
        {
            Version = version;
            Identity = identity;
            NewName = newName;
        }

    }

    public class Aggregate : EventSourcedAggregateRoot<AggregateIdentity, AggregateSnapshot>,
        IDomainEventHandler<AggregateCreated>,
        IDomainEventHandler<AggregateNameCreated>
    {
        public Aggregate(AggregateIdentity identity)
            : base(identity)
        {
            Raise(new AggregateCreated(identity));
        }

        public Aggregate(AggregateIdentity identity, IReadOnlyCollection<IEvent> events)
            : base(identity, events)
        {
            Id = identity;
            Mutate(events);
        }

        public Aggregate(AggregateIdentity identity, AggregateSnapshot snapshot, IReadOnlyCollection<IEvent> events)
            : base(identity, snapshot, events)
        {
            Id = identity;
            SetSnapshot(snapshot);
            Mutate(events);
        }

        private void SetSnapshot(AggregateSnapshot snapshot)
        {
            Id = new AggregateIdentity(snapshot.Id);
            _name = snapshot.Name;
        }

        private void Mutate(IReadOnlyCollection<IEvent> events)
        {
            foreach (dynamic @event in events)
            {
                Handle(@event);
            }
        }

        private string _name;

        public override AggregateSnapshot GetSnapshot()
        {
            return new AggregateSnapshot(Version, Id)
            {
                Id = Id.Id,
                Name = _name
            };
        }

        public void SetName(string name)
        {
            _name = name;
            Raise(new AggregateNameCreated(Id, _name));
        }

        public void Handle(AggregateCreated @event)
        {
            Id = @event.Identity;
        }

        public void Handle(AggregateNameCreated @event)
        {
            _name = @event.Name;
        }
    }

    public class AggregateCreated : IEvent
    {
        public AggregateIdentity Identity { get; private set; }

        public AggregateCreated(AggregateIdentity identity)
        {
            Identity = identity;
        }
    }

    public class AggregateNameCreated : IEvent
    {
        public AggregateIdentity Identity { get; private set; }

        public string Name { get; private set; }

        public AggregateNameCreated(AggregateIdentity identity, string name)
        {
            Identity = identity;
            Name = name;
        }
    }

    public class AggregateSnapshot : Snapshot
    {
        public AggregateSnapshot(int version, AggregateIdentity identity)
            : base(version, identity)
        { }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class AggregateIdentity : Identity
    {
        public readonly Guid Id;

        public AggregateIdentity(Guid id)
        {
            Id = id;
        }

        public override string GetId()
        {
            return Id.ToString();
        }

        public override string GetTag()
        {
            return "Aggregate";
        }
    }

}
