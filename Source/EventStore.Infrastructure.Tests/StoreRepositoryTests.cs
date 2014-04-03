using EventStore.Domain;
using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Tests.Arrange;
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
    public class StoreRepositoryTests
    {
        [Fact]
        public void repository_should_handle_user_created_event()
        {
            var eventStore = Substitute.For<IEventStore>();
            var cache = new RepositoryCache();
            StoreRepository repository = new StoreRepository(eventStore, cache);
            IEventHandler<DomainEvent> eventHandler = repository;

            var users = repository.GetAll<User>();
            users.Should().BeEmpty();

            var userCreated = UserEvents.ArrangeCreated();
            eventHandler.Handle(userCreated);

            users = repository.GetAll<User>();
            var user = users.FirstOrDefault();

            user.Should().NotBeNull();
            UserEvents.AssertUserCreated(user);
        }

        [Fact]
        public void repository_should_handle_two_users_created_event()
        {
            var eventStore = Substitute.For<IEventStore>();
            var cache = new RepositoryCache();
            StoreRepository repository = new StoreRepository(eventStore, cache);
            IEventHandler<DomainEvent> eventHandler = repository;

            var userCreated = UserEvents.ArrangeCreated();
            var secondCreated = UserEvents.ArrangeCreated(2);

            eventHandler.Handle(userCreated);
            eventHandler.Handle(secondCreated);

            var users = repository.GetAll<User>().ToList();
            users.Count.Should().Be(2);

            UserEvents.AssertUserCreated(users[0]);
            users[0].Id.Should().NotBe(users[1].Id);
        }

        [Fact]
        public void repository_should_save_user()
        {
            var eventStore = Substitute.For<IEventStore>();
            var cache = new RepositoryCache();
            
            StoreRepository repository = new StoreRepository(eventStore, cache);

            var user = User.CreateUser(UserEvents.TestLogin, UserEvents.TestPassword);
            repository.Save(user, Guid.NewGuid());

            cache.Get(user.Id).Should().Be(user);
            var saveEventsCall = eventStore.ReceivedCalls().First();
            var eventsToSave = saveEventsCall.GetArguments()[1].As<IEnumerable<DomainEvent>>();
            var userCreated = eventsToSave.First().As<EventStore.Messages.UserEvents.Created>();
            
            userCreated.Should().NotBeNull();
            userCreated.Login.Should().Be(UserEvents.TestLogin);
        }
    }
}
