using EventStore.ClientAPI;
using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Infrastructure.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class GetEventStore : IEventStore
    {
        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        private readonly IStoreSettings<IEventStoreConnection> _eventStoreSettings;
        private static readonly JsonSerializerSettings SerializerSettings;
        private static int originalVersion;
        private IServiceBus _serviceBus;

        static GetEventStore()
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
        }

        public GetEventStore(IStoreSettings<IEventStoreConnection> eventStoreSettings, IServiceBus serviceBus)
        {
            _eventStoreSettings = eventStoreSettings;
            _serviceBus = serviceBus;
        }

        public void FetchAllEvents()
        {
            var streamName = "main";

            var sliceStart = 0;
            StreamEventsSlice currentSlice;

            using (var _eventStoreConnection = _eventStoreSettings.GetConnection())
            {
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

                    foreach (var @event in currentSlice.Events)
                    {
                        var domainEvent = DeserializeEvent(@event.OriginalEvent.Metadata, @event.OriginalEvent.Data);

                        _serviceBus.Send(domainEvent);

                        originalVersion++;
                    }
                } while (!currentSlice.IsEndOfStream);

                _eventStoreConnection.Close();
            }
        }

        public void SaveEvents(IAggregate aggregate, IEnumerable<DomainEvent> newEvents, Guid commitId)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, commitId},
                {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };

            var streamName = "main";
            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e, commitHeaders)).ToList();

            using (var _eventStoreConnection = _eventStoreSettings.GetConnection())
            {
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

                _eventStoreConnection.Close();
            }
        }

        private static DomainEvent DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return (DomainEvent)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
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
    }
}
