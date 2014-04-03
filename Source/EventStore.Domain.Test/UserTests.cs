using EventStore.Messages.UserEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace EventStore.Domain.Test
{
    public class UserTests
    {
        private const string TestLogin = "test";
        private const string TestPassword = "test";

        [Fact]
        public void user_shold_apply_to_create_event()
        {
            var user = new User();
            var created = ArrangeCreated();

            user.Apply(created);

            AssertUserCreated(user);
        }

        [Fact]
        public void user_should_be_creatable()
        {
            var user = User.CreateUser(TestLogin, TestPassword);

            AssertUserCreated(user);
        }

        private static void AssertUserCreated(User user)
        {

            user.Login.Should().Be(TestLogin);
            user.Password.Should().Be(TestPassword);
            user.Id.Should().NotBe(Guid.Empty);
        }

        #region arranges

        private Created ArrangeCreated()
        {
            return new Created(TestLogin, TestPassword);
        }

        #endregion
    }
}
