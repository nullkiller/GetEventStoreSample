using EventStore.Commands.User;
using EventStore.Domain;
using EventStore.Domain.CommandHandlers;
using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Messages.UserEvents;
using EventStore.ReadModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EventStore.Web.Controllers
{
    public class AccountController : Controller
    {
        [Inject]
        public CommandHandler<CreateNewUserCommand> CreateNewUserHandler { get; set; }

        [Inject]
        public IRepository DataRepository { get; set; }

        public ActionResult Create()
        {
            return View(new UserDto());
        }

        [HttpPost]
        public ActionResult Create(UserDto user)
        {
            var createCommand = new CreateNewUserCommand(user.Login, user.Password);

            CreateNewUserHandler.Execute(createCommand);

            return View(user);
        }

        public ActionResult Login()
        {
            return View(new UserDto());
        }

        [HttpPost]
        public ActionResult Login(UserDto model)
        {
            var user = DataRepository.GetAll<User>().FirstOrDefault(u => u.Login == model.Login);

            if (user.Validate(model.Password))
            {
                FormsAuthentication.SetAuthCookie(user.Login, false);
                Response.Redirect("/");
            }

            return View(user);
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
            Response.Redirect("/");
        }
    }
}
