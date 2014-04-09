using EventStore.ClientAPI;
using EventStore.Domain.CommandHandlers;
using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Infrastructure.Events;
using EventStore.Infrastructure.Misc;
using EventStore.Infrastructure.Store;
using NEventStore;
using NEventStore.Serialization;
using Ninject.Modules;
using System.Data;
using System.Linq;

namespace EventStore.Infrastructure
{
    public class InfrastructureModule: NinjectModule
    {
        public override void Load()
        {
            Bind<IFormsAuthentication>().To<FormsAuthenticationWrapper>();
            Bind<IFileManager>().To<FileManager>();

            Bind<IServiceBus>().To<InProcessServiceBus>();

            Bind<ISerialize>().To<JsonSerializer>();
            Bind<IStoreSettings<IEventStoreConnection>>().To<StoreSettings>();
            Bind<IStoreSettings<IStoreEvents>>().To<NEventStoreSettings>();
            Bind<ISnapshotStore>().To<FileSnapshotStore>();
            Bind<IEventStore>().To<ByggEventStore>().InSingletonScope();

            Bind<IAggregateFactory>().To<AggregateFactory>();
            Bind<IIdentityGenerator>().To<IdentityGenerator>().InSingletonScope();
            Bind<IRepositoryCache, IProjection>().To<RepositoryCache>().InSingletonScope();
            Bind<IStoreSettings<IDbConnection>>().To<ByggStoreSettings>();
            Bind<IRepository>().To<StoreRepository>();

            RegisterHandlers();
        }

        public void RegisterHandlers()
        {
            var handlers = typeof(ICommandHandler<>)
                .Assembly
                .GetExportedTypes()
                .GroupBy(x => x.GetInterfaces().SingleOrDefault(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));

            foreach (var pair in handlers.Where(h => h.Key != null))
            {
                Bind(pair.Key).To(pair.FirstOrDefault());
            }
        }
    }
}
