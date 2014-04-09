using EventStore.Domain.Core;
using EventStore.Tests.Arrange;
using NEventStore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace EventStore.Infrastructure.Tests
{
    public class NEventStoreTests
    {
        [Fact]
        public void event_store_should_load_events()
        {
            var stream = Substitute.For<IEventStream>();
            var settings = FakeDatabase.ArrangeNSettings(stream);
            var testBus = new TestServiceBus();
            var snapshotStore = FakeDatabase.ArrangeSnapshotStore();
            
            var data = FakeDatabase.ArrangeAggregates().SelectMany(i => i.GetUncommittedEvents());
            var @event = data.First();

            stream.CommittedEvents.Returns(data.Select(i => new EventMessage { Body = i }).ToList());

            var store = new EventStore.Infrastructure.Store.NEventStore(settings, testBus);
            store.FetchAllEvents();

            testBus[0].Should().Be(@event);
        }

        [Fact]
        public void event_store_should_save_events()
        {
            var stream = Substitute.For<IEventStream>();
            var settings = FakeDatabase.ArrangeNSettings(stream);
            var testBus = new TestServiceBus();

            var @event = FakeUser.ArrangeCreated();
            var events = new List<DomainEvent> { @event };
            
            var store = new EventStore.Infrastructure.Store.NEventStore(settings, testBus);
            store.SaveEvents(events, Guid.NewGuid());

            testBus[0].Should().Be(@event);
        }
    }
}
