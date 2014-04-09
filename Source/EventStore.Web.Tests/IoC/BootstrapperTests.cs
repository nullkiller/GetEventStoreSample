using EventStore.Commands.User;
using EventStore.Domain.Core;
using EventStore.Infrastructure.Events;
using EventStore.Tests.Arrange;
using EventStore.Web.Controllers;
using EventStore.Web.Ioc;
using FluentAssertions;
using Xunit;

namespace EventStore.Web.Tests.IoC
{
    public class BootstrapperTests
    {
        [Fact]
        public void main_components_should_be_resolvable()
        {
            TestBootstrapper.Instance.Invoking(i =>
            {
                i.Get<IServiceBus>();
                i.Get<IRepository>();
            }).ShouldNotThrow();
        }

        [Fact]
        public void command_handlers_should_be_resolvable()
        {
            TestBootstrapper.Instance.Invoking(i =>
            {
                i.Get<ICommandHandler<CreateNewUserCommand>>();
            }).ShouldNotThrow();
        }

        [Fact]
        public void account_controller_should_be_resolvable()
        {
            TestBootstrapper.Instance.Invoking(i =>
            {
                var controller = i.Get<AccountController>();
                controller.DataRepository.Should().NotBeNull();
                controller.CreateNewUserHandler.Should().NotBeNull();
            }).ShouldNotThrow();
        }
    }
}
