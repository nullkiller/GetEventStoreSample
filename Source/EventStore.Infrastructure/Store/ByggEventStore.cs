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
    public class ByggEventStore: IEventStore
    {
        private const long SnapshotSize = 1000;

        private IStoreSettings<IDbConnection> _settings;
        private IServiceBus _serviceBus;
        private ISerialize _serializer;
        private ISnapshotStore _snapshotStore;
        private long lastId;
        private long snapshotVersion;
        private bool loaded;

        public ByggEventStore(IStoreSettings<IDbConnection> settings, IServiceBus serviceBus, ISerialize serializer, ISnapshotStore snapshotStore)
        {
            _settings = settings;
            _serviceBus = serviceBus;
            _serializer = serializer;
            _snapshotStore = snapshotStore;
        }

        public void FetchAllEvents()
        {
            lock (typeof(ByggEventStore))
            {
                if (loaded)
                {
                    return;
                }

                loaded = true;
            }

            var latestVersion = _snapshotStore.LoadSnapshot();

            snapshotVersion = latestVersion.LastEventId;
            lastId = latestVersion.LastEventId;

            using (var connection = _settings.GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "Select Id, Payload FROM Commits WHERE Id > @LastId ORDER BY Id";

                    command.Parameters.Add(new SqlParameter("@LastId", lastId));

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var payloadIndex = reader.GetOrdinal("Payload");
                        var idIndex = reader.GetOrdinal("Id");

                        while (reader.Read())
                        {
                            lastId = reader.GetInt64(idIndex);
                            var data = (byte[])reader.GetValue(payloadIndex);
                            var @event = _serializer.Deserialize<List<DomainEvent>>(data);

                            @event.ForEach(_serviceBus.Send);
                        }
                    }
                }
            }
        }

        public void SaveEvents(IAggregate aggregate, IEnumerable<DomainEvent> newEvents, Guid commitId)
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
                    lastId = (long)result;
                }
            }

            foreach (var @event in newEvents)
            {
                _serviceBus.Send(@event);
            }

            if (lastId > snapshotVersion + SnapshotSize)
            {
                lock (typeof(ByggEventStore))
                {
                    if (lastId > snapshotVersion + SnapshotSize)
                    {
                        snapshotVersion = lastId;
                        _snapshotStore.SaveSnapshot(new SnapshotVersion(snapshotVersion));
                    }
                }
            }
        }
    }
}
