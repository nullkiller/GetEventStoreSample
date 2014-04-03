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
        private static Dictionary<Guid, IAggregate> _all;
        private static Dictionary<Type, Func<IAggregate>> factoryMethods;
        private IEventStore _eventStore;

        static StoreRepository()
        {
            factoryMethods = new Dictionary<Type, Func<IAggregate>>
            {
                { typeof(EventStore.Messages.UserEvents.Created), () => new User() }
            };

            _all = new Dictionary<Guid, IAggregate>();
        }

        public StoreRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
        {
            return (TAggregate)_all[id];
        }
                
        public void Handle(DomainEvent @event)
        {
            var aggregate = GetOrAdd(@event.AggregateId, factoryMethods[@event.GetType()]);
            aggregate.ApplyEvent(@event);
        }

        private IAggregate GetOrAdd(Guid aggregateId, Func<IAggregate> aggregateFactory)
        {
            IAggregate existingAggregate;

            if (!_all.TryGetValue(aggregateId, out existingAggregate))
            {
                existingAggregate = aggregateFactory();
                _all.Add(aggregateId, existingAggregate);
            }

            return existingAggregate;
        }

        public TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate
        {
            throw new NotImplementedException();
        }

        public void Save(IAggregate aggregate, Guid commitId)
        {
            var newEvents = aggregate.GetUncommittedEvents().Cast<object>().ToList();

            _eventStore.SaveEvents(aggregate, newEvents, commitId);

            GetOrAdd(aggregate.Id, () => aggregate);
            
            aggregate.ClearUncommittedEvents();
        }

        public IQueryable<T> GetAll<T>()
             where T : class, IAggregate
        {
            return _all.Values.OfType<T>().AsQueryable();
        }
    }
}
