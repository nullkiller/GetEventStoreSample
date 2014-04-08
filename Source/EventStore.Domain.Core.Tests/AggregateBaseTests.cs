using EventStore.Messages.Employee;
using EventStore.Messages.UserEvents;
using EventStore.Tests.Arrange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace EventStore.Domain.Core.Tests
{
    public class AggregateBaseTests : AggregateBase, IRouteEvent<Created>, IRouteEvent<EmployeeCreated>
    {
        public DomainEvent LastAppliedEvent { get; private set; }

        [Fact]
        public void aggregate_base_should_raise_event()
        {
            var @event = FakeUser.ArrangeCreated();
            var aggregate = this.As<IAggregate>();

            aggregate.ClearUncommittedEvents();
            base.RaiseEvent(@event);

            var uncommitedEvents = aggregate.GetUncommittedEvents();
            uncommitedEvents.Single().Should().Be(@event);
        }

        [Fact]
        public void aggregate_base_should_call_apply()
        {
            DomainEvent @event = FakeUser.ArrangeCreated();
            base.RaiseEvent(@event);

            this.LastAppliedEvent.Should().Be(@event);

            @event = FakeEmployee.ArrangeCreated();
            base.RaiseEvent(@event);

            this.LastAppliedEvent.Should().Be(@event);
        }

        [Fact]
        public void aggregate_should_fail_if_no_route_found()
        {
            DomainEvent @event = FakeEmployee.ArrangeCompetenciesChanged(Guid.NewGuid());

            this.Invoking(a => base.RaiseEvent(@event)).ShouldThrow<HandlerForDomainEventNotFoundException>();
        }

        #region Routers

        void IRouteEvent<Created>.Apply(Created @event)
        {
            LastAppliedEvent = @event;
        }

        void IRouteEvent<EmployeeCreated>.Apply(EmployeeCreated @event)
        {
            LastAppliedEvent = @event;
        }

        #endregion
    }
}
