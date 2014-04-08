using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.Infrastructure.DataAccess
{
    public class InitialAggregateBuilder : IEventHandler<DomainEvent>
    {
        private IAggregateFactory _aggregateFactory;
        private IRepositoryCache _cache;

        public InitialAggregateBuilder(IAggregateFactory aggregateFactory, IRepositoryCache cache)
        {
            _aggregateFactory = aggregateFactory;
            _cache = cache;
        }

        public void Handle(DomainEvent @event)
        {
            IAggregate newAggregate = _aggregateFactory.CreateFrom(@event);
            IAggregate aggregate = _cache.Get(@event.AggregateId) ?? newAggregate;

            if (aggregate == null)
            {
                throw new AggregateVersionException(@event.AggregateId, @event.GetType(), 1, 1);
            }

            if (!aggregate.GetUncommittedEvents().Contains(@event))
            {
                aggregate.ApplyEvent(@event);

                if (newAggregate != null)
                {
                    if (Object.ReferenceEquals(newAggregate, aggregate))
                    {
                        _cache.Add(newAggregate);
                    }
                    else
                    {
                        throw new AggregateVersionException(@event.AggregateId, @event.GetType(), 1, 1);
                    }
                }
            }
        }
    }
}
