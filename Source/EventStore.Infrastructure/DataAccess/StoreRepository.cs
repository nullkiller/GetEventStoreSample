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
                { typeof(EventStore.Messages.UserEvents.Created), () => new User() },
                { typeof(EventStore.Messages.Employee.EmployeeCreated), () => new Employee() },
                { typeof(EventStore.Messages.CompetenceTree.CompetenceCreated), () => new Competence() }
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
            IAggregate aggregate;
            Func<IAggregate> factory;

            var isCreationEvent = factoryMethods.TryGetValue(@event.GetType(), out factory);

            if (isCreationEvent)
            {
                aggregate = factory();
            }
            else
            {
                aggregate = _cache.Get(@event.AggregateId);
            }

            aggregate.ApplyEvent(@event);

            if (isCreationEvent)
            {
                _cache.Add(aggregate);
            }
        }

        public void Save(IAggregate aggregate, Guid commitId)
        {
            var newEvents = aggregate.GetUncommittedEvents();

            var isNew = factoryMethods.ContainsKey(newEvents.First().GetType());
            if (isNew)
            {
                _cache.Add(aggregate);
            }

            _eventStore.SaveEvents(aggregate, newEvents, commitId);
            
            aggregate.ClearUncommittedEvents();
        }

        public IQueryable<T> GetAll<T>()
             where T : class, IAggregate
        {
            return _cache.GetAll().OfType<T>().AsQueryable();
        }
    }
}
