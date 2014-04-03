using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure
{
    public class ServiceBusModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IEventHandler<DomainEvent>>().To(typeof(StoreRepository));
        }
    }
}
