using EventStore.Domain.Core;
using EventStore.Infrastructure.Misc;
using EventStore.Tests.Arrange;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Domain.Tests.Commands
{
    public class UserCommandTests
    {
        [Fact]
        public void create_user_command_should_be_handled()
        {
            User createdUser = null;

            var repository = Substitute.For<IRepository>();
            repository.WhenForAnyArgs(i => i.Save(null, Guid.Empty)).Do(i => createdUser = i.Arg<User>());

            var command = new Domain.CommandHandlers.UserCommandHandler(repository, new IdentityGenerator());
            command.Execute(FakeUser.ArrangeCreatedCommand());

            FakeUser.AssertUserCreated(createdUser);
        }
    }
}
