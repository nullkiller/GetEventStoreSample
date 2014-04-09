using EventStore.Commands.User;
using EventStore.Domain.Core;
using EventStore.ReadModel;
using EventStore.Tests.Arrange;
using EventStore.Web.Controllers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using EventStore.ReadModel.Users;
using EventStore.Domain;
using System.Web.Mvc;
using EventStore.Infrastructure.Misc;

namespace EventStore.Web.Tests.Account
{
    public class AccountControllerTests
    {
        [Fact]
        public void account_controller_should_create_user()
        {
            CreateNewUserCommand receivedCommand = null;
            var controller = new AccountController();
            var handler = Substitute.For<ICommandHandler<CreateNewUserCommand>>();
            handler.WhenForAnyArgs(h => h.Execute(null)).Do(a => receivedCommand = a.Arg<CreateNewUserCommand>());
            controller.CreateNewUserHandler = handler;

            var dto = FakeUser.ArrangeUserDto();
            controller.Create(dto);

            receivedCommand.Should().NotBeNull();
        }

        [Fact]
        public void account_controller_should_login_user()
        {
            var repository = Substitute.For<IRepository>();
            var usersView = Substitute.For<IUsersView>();
            var authentication = Substitute.For<IFormsAuthentication>();

            bool authenticated = false;
            authentication.WhenForAnyArgs(a => a.SetAuthentication(null, false)).Do(i => authenticated = true);

            var controller = new AccountController();
            controller.DataRepository = repository;
            controller.UsersByLogin = usersView;
            controller.FormsAuthentication = authentication;

            var dto = FakeUser.ArrangeUserDto();
            var user = User.CreateUser(dto.Login, dto.Password, new ConstantIdentityGenerator(){ Identity = dto.AggregateRootId});

            usersView.GetUserId(dto.Login).Returns(dto.AggregateRootId);
            repository.GetById<User>(dto.AggregateRootId).Returns(user);

            var result = controller.Login(dto);

            result.Should().BeOfType<RedirectResult>();
            authenticated.Should().BeTrue();
        }

        [Fact]
        public void account_controller_should_not_login_invalid_user()
        {
            var repository = Substitute.For<IRepository>();
            var usersView = Substitute.For<IUsersView>();
            var authentication = Substitute.For<IFormsAuthentication>();

            bool authenticated = false;
            authentication.WhenForAnyArgs(a => a.SetAuthentication(null, false)).Do(i => authenticated = true);

            var controller = new AccountController();
            controller.DataRepository = repository;
            controller.UsersByLogin = usersView;
            controller.FormsAuthentication = authentication;

            var dto = FakeUser.ArrangeUserDto();

            usersView.GetUserId(dto.Login).Returns((Guid?)null);

            var result = controller.Login(dto);

            result.Should().BeOfType<ViewResult>();
            authenticated.Should().BeFalse();
        }
    }
}
