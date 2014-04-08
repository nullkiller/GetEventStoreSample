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
using EventStore.Infrastructure.Misc;

namespace EventStore.Infrastructure.Tests
{
    public class StoreRepositoryTests
    {
        [Fact]
        public void repository_should_save_user()
        {
            var eventStore = Substitute.For<IEventStore>();
            var cache = new RepositoryCache();
            
            StoreRepository repository = new StoreRepository(eventStore, cache, new AggregateFactory());

            var user = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, new IdentityGenerator());
            repository.Save(user, Guid.NewGuid());

            cache.Get(user.Id).Should().Be(user);

            var saveEventsCall = eventStore.ReceivedCalls().First();
            var eventsToSave = saveEventsCall.GetArguments()[1].As<IEnumerable<DomainEvent>>();
            var userCreated = eventsToSave.First().As<EventStore.Messages.UserEvents.Created>();
            
            userCreated.Should().NotBeNull();
            userCreated.Login.Should().Be(FakeUser.TestLogin);
        }

        [Fact]
        public void repository_shouldnt_save_users_with_same_id()
        {
            var eventStore = Substitute.For<IEventStore>();
            var identityGenerator = Substitute.For<IIdentityGenerator>();

            var guid = Guid.NewGuid();
            identityGenerator.NewId().Returns(i => guid);

            var cache = new RepositoryCache();

            StoreRepository repository = new StoreRepository(eventStore, cache, new AggregateFactory());

            var @event = FakeUser.ArrangeCreated();

            var user = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, identityGenerator);
            repository.Save(user, Guid.NewGuid());

            user = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, identityGenerator);
            repository.Invoking(i => i.Save(user, Guid.NewGuid())).ShouldThrow<AggregateVersionException>();
        }
    }
}
