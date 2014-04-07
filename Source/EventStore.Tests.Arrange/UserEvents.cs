using EventStore.Domain;
using EventStore.Messages.UserEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using EventStore.Infrastructure.Misc;

namespace EventStore.Tests.Arrange
{
    public class UserEvents
    {
        public const string TestLogin = "test";
        public static readonly string[] TestLogins = new string[] { TestLogin, TestLogin + 1, TestLogin + 2 };
        public const string TestPassword = "test";

        public static Created ArrangeCreated(int number = 0)
        {
            return new Created(TestLogins[number], TestPassword, new IdentityGenerator());
        }

        public static void AssertUserCreated(User user)
        {
            user.Login.Should().Be(TestLogin);
            user.Password.Should().Be(TestPassword);
            user.Id.Should().NotBe(Guid.Empty);
        }
    }
}
