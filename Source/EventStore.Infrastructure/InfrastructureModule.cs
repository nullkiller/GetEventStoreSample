using EventStore.ClientAPI;
using EventStore.Domain;
using EventStore.Domain.CommandHandlers;
using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Infrastructure.Events;
using EventStore.Infrastructure.Store;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure
{
    public class InfrastructureModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IStoreSettings>().To<StoreSettings>();
            Bind<IServiceBus>().To<InProcessServiceBus>();
            Bind<IEventStore>().To<GetEventStore>();
            Bind<IRepository>().To<StoreRepository>();

            RegisterHandlers();
        }

        public void RegisterHandlers()
        {
            var handlers = typeof(CommandHandler<>)
                .Assembly
                .GetExportedTypes()
                .GroupBy(x => x.GetInterfaces().SingleOrDefault(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(CommandHandler<>)));

            foreach (var pair in handlers.Where(h => h.Key != null))
            {
                Bind(pair.Key).To(pair.FirstOrDefault());
            }
        }
    }
}
