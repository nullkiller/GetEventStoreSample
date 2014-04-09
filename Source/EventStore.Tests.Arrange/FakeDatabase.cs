using EventStore.Domain.Core;
using EventStore.Infrastructure.Store;
using NEventStore;
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

        public static ISnapshotStore ArrangeSnapshotStore()
        {
            var store = Substitute.For<ISnapshotStore>();
            store.LoadSnapshot().Returns(new SnapshotVersion());

            return store;
        }

        public static IStoreSettings<NEventStore.IStoreEvents> ArrangeNSettings(IEventStream stream)
        {
            var settings = Substitute.For<IStoreSettings<IStoreEvents>>();
            settings.GetConnection().OpenStream(null, null, 0, 0).ReturnsForAnyArgs(stream);

            return settings;
        }

        public static IAggregate[] ArrangeAggregates()
        {
            var user = FakeUser.ArrangeUser();
            var competence = FakeCompetence.ArrangeCompetence();
            var employee = FakeEmployee.ArrangeEmployee(3);
            var Data = new IAggregate[] { user, competence, employee };
            return Data;
        }

        internal static IStoreSettings<IStoreEvents> ArrangeNSettings()
        {
            return ArrangeNSettings(Substitute.For<IEventStream>());
        }
    }
}
