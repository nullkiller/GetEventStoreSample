using EventStore.Domain.Core;
using EventStore.Infrastructure.Events;
using NEventStore;
using NEventStore.Dispatcher;
using NEventStore.Persistence.Sql.SqlDialects;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class CommitDispatcher : IDispatchCommits
    {
        private IServiceBus _serviceBus;

        public CommitDispatcher(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public void Dispatch(ICommit commit)
        {
            foreach (var @event in commit.Events)
            {
                _serviceBus.Send((DomainEvent)@event.Body);
            }
        }

        public void Dispose()
        {
            _serviceBus = null;
        }
    }

    public class NEventStoreSettings: IStoreSettings<IStoreEvents>
    {
        [Inject]
        public IServiceBus ServiceBus { get; set; }

        public IStoreEvents GetConnection()
        {
            return Wireup.Init()
                .UsingAsynchronousDispatchScheduler(new CommitDispatcher(ServiceBus))
                .UsingSqlPersistence("EventStore")
                .WithDialect(new MsSqlDialect())
                    .PageEvery(int.MaxValue)
                .Build();
        }
    }
}
