using CommonDomain.Persistence;
using EventStore.Commands.User;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.CommandHandlers
{
    public class UserCommandHandler: CommandHandler<CreateNewUserCommand>
    {
        [Inject]
        public IRepository Repository { get; set; }

        public void Execute(CreateNewUserCommand command)
        {
            var user = User.CreateUser(command.Login, command.Password);
            Repository.Save(user, Guid.NewGuid());
        }
    }
}
