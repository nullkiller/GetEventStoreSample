using EventStore.Infrastructure.Store;
using NEventStore.Serialization;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class FakeDatabase
    {
        public static IStoreSettings<IDbConnection> ArrangeSettings()
        {
            var settings = Substitute.For<IStoreSettings<IDbConnection>>();
            return settings;
        }

        public static ISerialize ArrangeSerializer<T>(T events)
        {
            var serializer = Substitute.For<ISerialize>();
            serializer.Deserialize<T>(null).ReturnsForAnyArgs(@events);

            return serializer;
        }

        public static DataTable ArrangeCommitsTable(byte amount)
        {
            var table = new DataTable("Commits");

            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Payload", typeof(byte[]));

            for (byte i = 0; i < amount; i++)
            {
                table.Rows.Add(1, new byte[] { i });
            }

            return table;
        }

        public static IDbConnection ArrangeConnection()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var parameters = Substitute.For<IDataParameterCollection>();

            connection.CreateCommand().Returns(command);
            command.Parameters.Returns(parameters);

            return connection;
        }

        public static ISnapshotStore ArrangeSnapshotStore()
        {
            var store = Substitute.For<ISnapshotStore>();
            store.LoadSnapshot().Returns(new SnapshotVersion());

            return store;
        }
    }
}
