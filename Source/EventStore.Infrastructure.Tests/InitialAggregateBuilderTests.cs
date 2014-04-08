using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Tests.Arrange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using EventStore.Domain;

namespace EventStore.Infrastructure.Tests
{
    public class InitialAggregateBuilderTests
    {
        [Fact]
        public void initial_buider_should_handle_creation_event()
        {
            var cache = new RepositoryCache();
            IEventHandler<DomainEvent> eventHandler = new InitialAggregateBuilder(new AggregateFactory(), cache);

            for (var i = 0; i < 3; i++)
            {
                var userCreated = FakeUser.ArrangeCreated();
                eventHandler.Handle(userCreated);

                var user = (User)cache.Get(userCreated.AggregateId);

                user.Should().NotBeNull();
                FakeUser.AssertUserCreated(user);
            }
        }

        [Fact]
        public void initial_buider_should_handle_two_users_created_event()
        {
            var cache = new RepositoryCache();
            IEventHandler<DomainEvent> eventHandler = new InitialAggregateBuilder(new AggregateFactory(), cache);

            var userCreated = FakeUser.ArrangeCreated();
            var secondCreated = FakeUser.ArrangeCreated(2);

            eventHandler.Handle(userCreated);
            eventHandler.Handle(secondCreated);

            var users = cache.GetAll().ToList();
            users.Count.Should().Be(2);

            FakeUser.AssertUserCreated((User)cache.Get(userCreated.AggregateId));
            users[0].Id.Should().NotBe(users[1].Id);
        }

        [Fact]
        public void initial_buider_should_handle_change_event()
        {
            var cache = new RepositoryCache();

            IEventHandler<DomainEvent> eventHandler = new InitialAggregateBuilder(new AggregateFactory(), cache);

            var created = FakeEmployee.ArrangeCreated();
            var competencies = FakeEmployee.ArrangeCompetenciesChanged(created.AggregateId);

            eventHandler.Handle(created);
            eventHandler.Handle(competencies);

            var employee = cache.Get(created.AggregateId).As<Employee>();
            employee.Should().NotBeNull();
            employee.Id.Should().Be(created.AggregateId);
            employee.Competencies.Select(i => i.CompetenceId).Should().BeEquivalentTo(competencies.Competencies.Select(i => i.CompetenceId));
        }

        [Fact]
        public void initial_bilder_should_skip_uncommited_events()
        {
            var employee = FakeEmployee.ArrangeEmployee(2);
            var newCompetences = FakeEmployee.ArrangeCompetencies(3);
            employee.AddCompetences(newCompetences);

            var events = ((IAggregate)employee).GetUncommittedEvents().ToList();

            var cache = new RepositoryCache();
            IEventHandler<DomainEvent> eventHandler = new InitialAggregateBuilder(new AggregateFactory(), cache);

            cache.Add(employee);

            var firstCompetenceChange = events[1];
            eventHandler.Handle(firstCompetenceChange);

            employee.Competencies.Count.Should().Be(5);
        }

        [Fact]
        public void initial_builder_should_fail_if_no_aggregate_found()
        {
            var employee = FakeEmployee.ArrangeEmployee(2);
            var newCompetences = FakeEmployee.ArrangeCompetencies(3);
            employee.AddCompetences(newCompetences);

            var events = ((IAggregate)employee).GetUncommittedEvents().ToList();

            var cache = new RepositoryCache();
            IEventHandler<DomainEvent> eventHandler = new InitialAggregateBuilder(new AggregateFactory(), cache);
            
            var firstCompetenceChange = events[1];
            eventHandler.Invoking(i => i.Handle(firstCompetenceChange)).ShouldThrow<AggregateVersionException>();
        }

        [Fact]
        public void initial_builder_should_fail_for_creation_event_for_existing_aggregate()
        {
            var employee = FakeEmployee.ArrangeEmployee(2);
            var newCompetences = FakeEmployee.ArrangeCompetencies(3);
            employee.AddCompetences(newCompetences);

            var events = ((IAggregate)employee).GetUncommittedEvents().ToList();

            var cache = new RepositoryCache();
            IEventHandler<DomainEvent> eventHandler = new InitialAggregateBuilder(new AggregateFactory(), cache);

            cache.Add(employee);

            var creationEvent = events[0];
            eventHandler.Invoking(i => i.Handle(creationEvent)).ShouldNotThrow<AggregateVersionException>();
        }
    }
}
