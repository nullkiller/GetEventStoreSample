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
    public class StoreRepository : IRepository
    {
        private IEventStore _eventStore;
        private IRepositoryCache _cache;
        private IAggregateFactory _factory;
                
        public StoreRepository(IEventStore eventStore, IRepositoryCache cache, IAggregateFactory factory)
        {
            _eventStore = eventStore;
            _cache = cache;
            _factory = factory;
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return (TAggregate)_cache.Get(id);
        }

        public void Save(IAggregate aggregate, Guid commitId)
        {
            var newEvents = aggregate.GetUncommittedEvents();

            if (_factory.IsCreationEvent(newEvents.First()))
            {
                _cache.Add(aggregate);
            }

            _eventStore.SaveEvents(newEvents, commitId);
            
            aggregate.ClearUncommittedEvents();
        }

        public IQueryable<T> GetAll<T>()
             where T : class, IAggregate
        {
            return _cache.GetAll().OfType<T>().AsQueryable();
        }
    }
}
