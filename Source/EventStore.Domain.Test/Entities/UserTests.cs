using EventStore.Messages.UserEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using EventStore.Tests.Arrange;
using EventStore.Infrastructure.Misc;
using EventStore.Domain.Core;

namespace EventStore.Domain.Tests
{
    public class UserTests
    {
        [Fact]
        public void user_shold_apply_to_create_event()
        {
            var user = new User();
            var created = FakeUser.ArrangeCreated();

            user.As<IRouteEvent<Created>>().Apply(created);

            FakeUser.AssertUserCreated(user);
        }

        [Fact]
        public void user_should_be_creatable()
        {
            var user = User.CreateUser(FakeUser.TestLogin, FakeUser.TestPassword, new IdentityGenerator());

            FakeUser.AssertUserCreated(user);
        }
    }
}
