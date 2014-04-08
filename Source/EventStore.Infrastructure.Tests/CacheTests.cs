using EventStore.Domain;
using EventStore.Infrastructure.DataAccess;
using EventStore.Tests.Arrange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using EventStore.Domain.Core;
using EventStore.Infrastructure.Misc;
using NSubstitute;

namespace EventStore.Infrastructure.Tests
{
    public class CacheTests
    {
        [Fact]
        public void cache_should_save_data()
        {
            var cache = new RepositoryCache();
            var aggregate = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, new IdentityGenerator());

            cache.Add(aggregate);
            cache.Get(aggregate.Id).Should().Be(aggregate);
        }

        [Fact]
        public void cache_should_save_multiple_data()
        {
            var cache = new RepositoryCache();

            IAggregate aggregate = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, new IdentityGenerator());
            cache.Add(aggregate);

            aggregate = Competence.Create("test", new IdentityGenerator());
            cache.Add(aggregate);

            cache.Get(aggregate.Id).Should().Be(aggregate);
        }

        [Fact]
        public void cache_should_not_allow_duplicate_add()
        {
            var id = Guid.NewGuid();
            var generator = Substitute.For<IIdentityGenerator>();
            generator.NewId().Returns(id);

            var cache = new RepositoryCache();

            var aggregate = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, generator);
            cache.Add(aggregate);

            aggregate = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, generator);
            cache.Invoking(c => c.Add(aggregate)).ShouldThrow<AggregateVersionException>();
        }

        [Fact]
        public void cache_should_return_null_if_nothing_found()
        {
            var cache = new RepositoryCache();
            var notExistingId = Guid.NewGuid();
            cache.Get(notExistingId).Should().BeNull();
        }
    }
}
