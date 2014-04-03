using CommonDomain.Persistence;
using EventStore.ClientAPI;
using EventStore.Domain;
using EventStore.Domain.CommandHandlers;
using EventStore.Infrastructure.DataAccess;
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
            var ip = new IPEndPoint(new IPAddress(new byte[]{127, 0, 0, 1}), 1113);

            Bind<IEventStoreConnection>().ToMethod(context => EventStoreConnection.Create(ip));
            Bind<IRepository>().To<GetEventStoreRepository>();
            Bind<IReadRepository>().To<GetEventStoreRepository>();

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
