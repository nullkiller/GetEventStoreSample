using EventStore.Domain.Core;
using EventStore.ReadModel.Users;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.ReadModel
{
    public class ProjectionsModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IUsersView, IProjection, IEventHandler<EventStore.Messages.UserEvents.Created>>().To<UsersView>().InSingletonScope();
        }
    }
}
