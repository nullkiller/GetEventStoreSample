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
    public class FakeUser
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

        public static EventStore.Commands.User.CreateNewUserCommand ArrangeCreatedCommand()
        {
            return new Commands.User.CreateNewUserCommand(TestLogin, TestPassword);
        }

        public static EventStore.ReadModel.UserDto ArrangeUserDto(int number = 0)
        {
            return new ReadModel.UserDto { Login = TestLogins[number], Password = TestPassword, AggregateRootId = Guid.NewGuid() };
        }

        public static User ArrangeUser()
        {
            return User.CreateUser(TestLogin, TestPassword, new IdentityGenerator());
        }
    }
}
