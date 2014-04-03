using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using EventStore.Domain;
using EventStore.Messages;
using Ninject;
using EventStore.Domain.Core;
using EventStore.Infrastructure.Store;

namespace EventStore.Infrastructure.DataAccess
{
    public class StoreRepository : IRepository, IEventHandler<DomainEvent>
    {
        private static Dictionary<Type, Func<IAggregate>> factoryMethods;
        private IEventStore _eventStore;
        private IRepositoryCache _cache;

        static StoreRepository()
        {
            factoryMethods = new Dictionary<Type, Func<IAggregate>>
            {
                { typeof(EventStore.Messages.UserEvents.Created), () => new User() }
            };
        }

        public StoreRepository(IEventStore eventStore, IRepositoryCache cache)
        {
            _eventStore = eventStore;
            _cache = cache;
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return (TAggregate)_cache.Get(id);
        }
                
        void IEventHandler<DomainEvent>.Handle(DomainEvent @event)
        {
            var aggregate = _cache.GetOrAdd(@event.AggregateId, factoryMethods[@event.GetType()]);
            aggregate.ApplyEvent(@event);
        }

        public void Save(IAggregate aggregate, Guid commitId)
        {
            var newEvents = aggregate.GetUncommittedEvents();

            _eventStore.SaveEvents(aggregate, newEvents, commitId);

            _cache.GetOrAdd(aggregate.Id, () => aggregate);
            
            aggregate.ClearUncommittedEvents();
        }

        public IQueryable<T> GetAll<T>()
             where T : class, IAggregate
        {
            return _cache.GetAll().OfType<T>().AsQueryable();
        }
    }
}
