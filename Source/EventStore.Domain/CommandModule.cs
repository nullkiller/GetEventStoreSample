using EventStore.Domain.CommandHandlers;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain
{
    public class CommandModule: NinjectModule
    {
        public override void Load()
        {
            Bind<CommandHandler<EventStore.Commands.User.CreateNewUserCommand>>().To<UserCommandHandler>();
        }
    }
}
