using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using CommonDomain.Persistence;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using EventStore.Domain;
using EventStore.Messages;
using Ninject;

namespace EventStore.Infrastructure.DataAccess
{
    public class GetEventStoreRepository : IRepository, IReadRepository
    {
        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        private readonly IEventStoreConnection _eventStoreConnection;
        private static readonly JsonSerializerSettings SerializerSettings;

        private static Dictionary<Guid, IAggregate> _all;
        private static Dictionary<Type, Func<IAggregate>> factoryMethods;
        private static int originalVersion; 

        static GetEventStoreRepository()
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

            factoryMethods = new Dictionary<Type, Func<IAggregate>>
            {
                { typeof(EventStore.Messages.UserEvents.Created), () => new User() }
            };

            _all = new Dictionary<Guid, IAggregate>();
        }

        public GetEventStoreRepository(IEventStoreConnection eventStoreConnection)
        {
            _eventStoreConnection = eventStoreConnection;
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return (TAggregate)_all[id];
        }

        public void Load()
        {
            var streamName = "main";

            var sliceStart = 0;
            StreamEventsSlice currentSlice;

            _eventStoreConnection.Connect();

            originalVersion = ExpectedVersion.NoStream;

            do
            {
                currentSlice = _eventStoreConnection.ReadStreamEventsForward(streamName, sliceStart, ReadPageSize, false);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                {
                    return;
                }

                if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                {
                    throw new StoreNotFoundException("main");
                }
                
                sliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    var @event = DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data);

                    var aggregate = GetOrAdd(@event.AggregateId, factoryMethods[@event.GetType()]);
                    aggregate.ApplyEvent(@event);

                    originalVersion++;
                }
            } while (!currentSlice.IsEndOfStream);

            _eventStoreConnection.Close();
        }

        private IAggregate GetOrAdd(Guid aggregateId, Func<IAggregate> aggregateFactory)
        {
            IAggregate existingAggregate;

            if (!_all.TryGetValue(aggregateId, out existingAggregate))
            {
                existingAggregate = aggregateFactory();
                _all.Add(aggregateId, existingAggregate);
            }

            return existingAggregate;
        }

        public TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate
        {
            throw new NotImplementedException();
        }

        private static DomainEvent DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return (DomainEvent)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }

        public void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, commitId},
                {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };

            updateHeaders(commitHeaders);

            var streamName = "main";
            var newEvents = aggregate.GetUncommittedEvents().Cast<object>().ToList();

            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e, commitHeaders)).ToList();
            
            _eventStoreConnection.Connect();

            if (eventsToSave.Count < WritePageSize)
            {
                _eventStoreConnection.AppendToStream(streamName, originalVersion, eventsToSave);
                originalVersion += eventsToSave.Count();
            }
            else
            {
                var transaction = _eventStoreConnection.StartTransaction(streamName, originalVersion);

                var position = 0;
                while (position < eventsToSave.Count)
                {
                    var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);

                    transaction.Write(pageEvents);
                    position += WritePageSize;

                    originalVersion += pageEvents.Count();
                }

                transaction.Commit();
            }

            GetOrAdd(aggregate.Id, () => aggregate);

            _eventStoreConnection.Close();

            aggregate.ClearUncommittedEvents();
        }

        private static EventData ToEventData(Guid eventId, object evnt, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {
                    EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName
                }
            };

            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = evnt.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }

        public IQueryable<T> GetAll<T>()
             where T : class, IAggregate
        {
            return _all.Values.OfType<T>().AsQueryable();
        }
    }
}
