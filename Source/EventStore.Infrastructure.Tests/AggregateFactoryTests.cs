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
    public class AggregateFactoryTests
    {
        [Fact]
        public void factory_should_create_user()
        {
            var userCreated = FakeUser.ArrangeCreated();
            var factory = new AggregateFactory();

            var user = factory.CreateFrom(userCreated).As<User>();

            user.Should().NotBeNull();
            FakeUser.AssertUserCreated(user);
        }

        [Fact]
        public void factory_should_return_null()
        {
            var competenciesChanged = FakeEmployee.ArrangeCompetenciesChanged(Guid.NewGuid());
            var factory = new AggregateFactory();

            var employee = factory.CreateFrom(competenciesChanged);

            employee.Should().BeNull();
        }
    }
}
