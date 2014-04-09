using EventStore.Domain.Core;
using EventStore.Infrastructure.Store;
using EventStore.Tests.Arrange;
using NEventStore.Serialization;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Data.SqlClient;
using System.Data.Common;

namespace EventStore.Infrastructure.Tests
{
    public class ByggEventStoreTests
    {
        [Fact]
        public void event_store_should_load_events()
        {
            var data = FakeDatabase.ArrangeCommitsTable(1);
            var settings = FakeDatabase.ArrangeSettings();
            var testBus = new TestServiceBus();
            var snapshotStore = FakeDatabase.ArrangeSnapshotStore();

            var @event = FakeUser.ArrangeCreated();
            var events = new List<DomainEvent> { @event };
            var serializer = FakeDatabase.ArrangeSerializer(events);

            settings.GetConnection().CreateCommand().ExecuteReader().Returns(data.CreateDataReader());

            var store = new ByggEventStore(settings, testBus, serializer, snapshotStore);
            store.FetchAllEvents();

            testBus[0].Should().Be(@event);
        }

        [Fact]
        public void event_store_should_save_events()
        {
            var settings = FakeDatabase.ArrangeSettings();
            var testBus = new TestServiceBus();

            var @event = FakeUser.ArrangeCreated();
            var events = new List<DomainEvent> { @event };
            var serializer = FakeDatabase.ArrangeSerializer(events);
            var snapshotStore = FakeDatabase.ArrangeSnapshotStore();

            settings.GetConnection().CreateCommand().ExecuteScalar().ReturnsForAnyArgs((decimal)1);

            var store = new ByggEventStore(settings, testBus, serializer, snapshotStore);
            store.SaveEvents(null, events, Guid.NewGuid());

            testBus[0].Should().Be(@event);
        }

        [Fact]
        public void event_store_should_save_snapshot()
        {
            var settings = FakeDatabase.ArrangeSettings();
            var testBus = new TestServiceBus();

            var @event = FakeUser.ArrangeCreated();
            var events = new List<DomainEvent> { @event };
            var serializer = FakeDatabase.ArrangeSerializer(events);
            var snapshotStore = FakeDatabase.ArrangeSnapshotStore();

            var snapshotSaved = false;

            snapshotStore.WhenForAnyArgs(s => s.SaveSnapshot(null)).Do(i => snapshotSaved = true);
            settings.GetConnection().CreateCommand().ExecuteScalar().ReturnsForAnyArgs((decimal)10000);

            var store = new ByggEventStore(settings, testBus, serializer, snapshotStore);
            store.SaveEvents(null, events, Guid.NewGuid());

            snapshotSaved.Should().BeTrue();
        }
    }
}
