using EventStore.Domain.Core;
using EventStore.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventStore.Serialization;
using System.Data.SqlClient;

namespace EventStore.Infrastructure.Store
{
    public class SimpleEventStore: IEventStore
    {
        private const long SnapshotSize = 1000;

        private IStoreSettings<IDbConnection> _settings;
        private IServiceBus _serviceBus;
        private ISerialize _serializer;
        private ISnapshotStore _snapshotStore;
        private long _lastId;
        private long _snapshotVersion;
        private bool _loaded;

        public SimpleEventStore(IStoreSettings<IDbConnection> settings, IServiceBus serviceBus, ISerialize serializer, ISnapshotStore snapshotStore)
        {
            _settings = settings;
            _serviceBus = serviceBus;
            _serializer = serializer;
            _snapshotStore = snapshotStore;
        }

        public void FetchAllEvents()
        {
            lock (typeof(SimpleEventStore))
            {
                if (_loaded)
                {
                    return;
                }

                _loaded = true;
            }

            LoadLatestSnapshot();

            LoadRestEvents();
        }

        public void SaveEvents(IEnumerable<DomainEvent> newEvents, Guid commitId)
        {
            PersistEvents(newEvents);

            NotifyBus(newEvents);

            UnsureSnapshot();
        }

        private void LoadRestEvents()
        {
            using (var connection = _settings.GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "Select Id, Payload FROM Commits WHERE Id > @LastId ORDER BY Id";

                    command.Parameters.Add(new SqlParameter("@LastId", _lastId));

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var payloadIndex = reader.GetOrdinal("Payload");
                        var idIndex = reader.GetOrdinal("Id");

                        while (reader.Read())
                        {
                            _lastId = reader.GetInt64(idIndex);
                            var data = (byte[])reader.GetValue(payloadIndex);
                            var @event = _serializer.Deserialize<List<DomainEvent>>(data);

                            @event.ForEach(_serviceBus.Send);
                        }
                    }
                }
            }
        }

        private void LoadLatestSnapshot()
        {
            var latestVersion = _snapshotStore.LoadSnapshot();

            _snapshotVersion = latestVersion.LastEventId;
            _lastId = latestVersion.LastEventId;
        }

        private void UnsureSnapshot()
        {
            if (_lastId > _snapshotVersion + SnapshotSize)
            {
                lock (typeof(SimpleEventStore))
                {
                    if (_lastId > _snapshotVersion + SnapshotSize)
                    {
                        _snapshotVersion = _lastId;
                        _snapshotStore.SaveSnapshot(new SnapshotVersion(_snapshotVersion));
                    }
                }
            }
        }

        private void NotifyBus(IEnumerable<DomainEvent> newEvents)
        {
            foreach (var @event in newEvents)
            {
                _serviceBus.Send(@event);
            }
        }

        private void PersistEvents(IEnumerable<DomainEvent> newEvents)
        {
            using (var connection = _settings.GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT Commits(Payload) VALUES (@Payload); SELECT @@IDENTITY";

                    byte[] data = _serializer.Serialize(newEvents);
                    var parameter = new SqlParameter("@Payload", SqlDbType.VarBinary, data.Length);
                    parameter.Value = data;
                    command.Parameters.Add(parameter);

                    connection.Open();

                    var result = (decimal)command.ExecuteScalar();
                    _lastId = (long)result;
                }
            }
        }
    }
}
