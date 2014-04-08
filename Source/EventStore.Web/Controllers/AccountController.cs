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

            if (user.CheckPassword(model.Password))
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

/*
 * • Competence tree categories: 30
• Competences: 100
• Individuals count: 300 000
• Average number of competences per individual: 10
• Construction and Education companies: 1 000 
• Application Providers count: 6
• Amount of people coming to industry per month: 2 000 - 4 000
• Amount of people changing the employee per month: 1 000
Finland
• Competence tree categories: 15
• Competences: 50
• Individuals count: 150 000
• Average number of competences per individual: 10
• Construction and Education companies: 500 
• Application Providers count: 15
• Amount of people coming to industry per month: 1 000 - 2 000
• Amount of people changing the employee per month: 500*/
