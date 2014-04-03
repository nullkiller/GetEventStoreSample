using EventStore.Messages.UserEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using EventStore.Tests.Arrange;

namespace EventStore.Domain.Tests
{
    public class UserTests
    {

        [Fact]
        public void user_shold_apply_to_create_event()
        {
            var user = new User();
            var created = UserEvents.ArrangeCreated();

            user.Apply(created);

            UserEvents.AssertUserCreated(user);
        }

        [Fact]
        public void user_should_be_creatable()
        {
            var user = User.CreateUser(UserEvents.TestLogin, UserEvents.TestPassword);

            UserEvents.AssertUserCreated(user);
        }
    }
}
