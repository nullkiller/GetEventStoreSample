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
        private IStoreSettings<IDbConnection> _settings;
        private IServiceBus _serviceBus;
        private ISerialize _serializer;

        public ByggEventStore(IStoreSettings<IDbConnection> settings, IServiceBus serviceBus, ISerialize serializer)
        {
            _settings = settings;
            _serviceBus = serviceBus;
            _serializer = serializer;
        }

        public void FetchAllEvents()
        {
            using (var connection = _settings.GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "Select Payload FROM Commits";

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var index = reader.GetOrdinal("Payload");

                        while (reader.Read())
                        {
                            var data = (byte[])reader.GetValue(index);
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
                    command.CommandText = "INSERT Commits(Payload) VALUES (@Payload)";

                    byte[] data = _serializer.Serialize(newEvents);
                    var parameter = new SqlParameter("@Payload", SqlDbType.VarBinary, data.Length);
                    parameter.Value = data;
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            foreach (var @event in newEvents)
            {
                _serviceBus.Send(@event);
            }
        }
    }
}
