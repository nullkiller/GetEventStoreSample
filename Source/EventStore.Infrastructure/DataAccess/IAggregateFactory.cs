using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.Infrastructure.DataAccess
{
    public interface IAggregateFactory
    {
        IAggregate CreateFrom(DomainEvent @event);
        bool IsCreationEvent(DomainEvent @event);
    }
}
